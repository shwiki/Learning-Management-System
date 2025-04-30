using System;
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
        private ApplicationUserManager _userManager;

        public AdminController() { }

        public AdminController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext
                          .GetOwinContext()
                          .GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        // GET: /Admin
        // You’ll need a Views/Admin/Index.cshtml – even if it’s just a confirmation page
        public ActionResult Index()
        {
            // 1) load all users into memory
            var allUsers = UserManager.Users.ToList();

            // 2) only then filter by role
            var teachers = allUsers
                .Where(u => UserManager.IsInRole(u.Id, "Teacher"))
                .ToList();

            return View(teachers);
        }

        // GET: /Admin/CreateTeacher
        public ActionResult CreateTeacher()
            => View(new CreateTeacherViewModel());

        // POST: /Admin/CreateTeacher
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTeacher(CreateTeacherViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // build the new teacher user
            var teacher = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,           // skip email-confirm if you like
                ContactNumber = model.ContactNumber,
                ClassName = model.ClassName
            };

            // 1) generate a secure temp password
            var tempPassword = IdentityHelper.GenerateSecureTempPassword();

            // 2) create with that temp password
            var createResult = await UserManager.CreateAsync(teacher, tempPassword);
            if (!createResult.Succeeded)
            {
                AddErrors(createResult);
                return View(model);
            }

            // 3) assign Teacher role
            await UserManager.AddToRoleAsync(teacher.Id, "Teacher");

            // 4) generate a reset-password link
            var token = await UserManager.GeneratePasswordResetTokenAsync(teacher.Id);
            var callbackUrl = Url.Action(
                "ResetPassword", "Account",
                new { userId = teacher.Id, code = token },
                protocol: Request.Url.Scheme);

            System.Diagnostics.Debug.WriteLine($"[Admin] EmailService is {(UserManager.EmailService == null ? "NULL" : "NOT NULL")}");

            var messageBody = $@"
                Hello {teacher.Email},<br/><br/>
                Your LMS account has been created. Here are your credentials and next steps:<br/>
                <ul>
                  <li><b>Username:</b> {teacher.Email}</li>
                  <li><b>Temporary password:</b> {tempPassword}</li>
                </ul>
                Please <a href=""{callbackUrl}"">click here</a> to set your permanent password.<br/>
                After logging in, go to <strong>Manage Account → Change Password</strong> to finalize your password, 
                and then to <strong>Manage Account → Edit Profile</strong> to update your contact info.<br/><br/>
                Thanks,<br/>LMS Admin Team";

            // DEBUG: Who really owns EmailService?
            var svc = UserManager.EmailService;
            System.Diagnostics.Debug.WriteLine($"[Admin] EmailService instance: {svc?.GetType().FullName}");

            // DEBUG: before we await…
            System.Diagnostics.Debug.WriteLine("[Admin] about to call SendAsync");

            // ACTUAL send
            await svc.SendAsync(new IdentityMessage
            {
                Destination = teacher.Email,
                Subject = "Your new LMS account",
                Body = messageBody
            });

            // DEBUG: after we await
            System.Diagnostics.Debug.WriteLine("[Admin] SendAsync completed");


            // 6) redirect to Index (avoid the 404 on /Admin)
            TempData["Success"] = $"Teacher {teacher.Email} created and emailed.";
            return RedirectToAction("Index");
        }

        // secure password generator
        public static class IdentityHelper
        {
            public static string GenerateSecureTempPassword()
            {
                const int length = 12;
                const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";

                var bytes = new byte[length];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(bytes);
                }

                var chars = bytes.Select(b => valid[b % valid.Length]).ToArray();
                return new string(chars);
            }
        }


        #region Helpers
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);
        }
        #endregion
    }
}
