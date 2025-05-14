using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using sys.Models;
using sys.Services;

namespace sys.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
            => _userManager ?? (_userManager = HttpContext.GetOwinContext()
                .GetUserManager<ApplicationUserManager>());

        // GET: /Admin
        public ActionResult Index()
        {
            // 1) Get the Teacher role’s ID
            var teacherRoleId = _db.Roles
                .Where(r => r.Name == "Teacher")
                .Select(r => r.Id)
                .Single();

            // 2) Find all users who have that RoleId in their Roles collection
            var teachers = _db.Users
                .Where(u => u.Roles.Any(ur => ur.RoleId == teacherRoleId))
                .ToList();

            return View(teachers);
        }

        // GET: /Admin/CreateTeacher
        public ActionResult CreateTeacher()
            => View(new CreateTeacherViewModel());

        // POST: /Admin/CreateTeacher
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTeacher(CreateTeacherViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // --- 1) Handle optional photo upload ---
            if (model.PhotoUpload != null && model.PhotoUpload.ContentLength > 0)
            {
                // a) ensure the upload folder exists
                var uploadDir = Server.MapPath("~/Uploads/Profiles");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                // b) pick a unique filename and save
                var fileName = Path.GetFileName(model.PhotoUpload.FileName);
                var unique = $"{Guid.NewGuid()}_{fileName}";
                var fullPath = Path.Combine(uploadDir, unique);
                model.PhotoUpload.SaveAs(fullPath);

                // c) store the web‐relative path so you can render <img src="..." />
                model.PhotoPath = "/Uploads/Profiles/" + unique;
            }

            // --- 2) Create the identity user ---
            var teacherUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                ContactNumber = model.ContactNumber,
                ClassName = model.ClassName,
                PhotoPath = model.PhotoPath    // for profile display later
            };

            // 2a) Generate and assign a temporary password
            var tempPwd = IdentityHelper.GenerateSecureTempPassword();
            var createResult = await UserManager.CreateAsync(teacherUser, tempPwd);
            if (!createResult.Succeeded)
            {
                AddErrors(createResult);
                return View(model);
            }

            // --- 3) Assign the "Teacher" role ---
            await UserManager.AddToRoleAsync(teacherUser.Id, "Teacher");

            // --- 4) Email them a reset-link so they can set a permanent password ---
            var token = await UserManager.GeneratePasswordResetTokenAsync(teacherUser.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account",
                new { userId = teacherUser.Id, code = token }, Request.Url.Scheme);

            await UserManager.EmailService.SendAsync(new IdentityMessage
            {
                Destination = model.Email,
                Subject = "Your new LMS account is ready!",
                Body = $@"
            Hello {model.Email},<br/><br/>
            Your LMS account has been created.<br/>
            Username: <b>{model.Email}</b><br/>
            Temporary password: <b>{tempPwd}</b><br/><br/>
            Please <a href=""{callbackUrl}"">click here to set your permanent password</a>."
            });

            // --- 5) Persist the same VM into your Teachers table ---
            _db.CreateTeacherViewModel.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Teacher {model.Email} created and emailed.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
                _userManager?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helpers
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);
        }

        public static class IdentityHelper
        {
            public static string GenerateSecureTempPassword()
            {
                const int length = 12;
                const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
                var bytes = new byte[length];
                using (var rng = new RNGCryptoServiceProvider())
                    rng.GetBytes(bytes);
                var chars = bytes.Select(b => valid[b % valid.Length]).ToArray();
                return new string(chars);
            }
        }
        #endregion
    }
}
