using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using sys.Models;
using sys.Models.Student;
using sys.Helpers;
using static sys.Models.Student.PendingUser;
using sys.Models.Approval;

namespace sys.Controllers.Approval
{
    public class PendingUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PendingUsers/WaitingApproval
        public ActionResult WaitingApproval()
        {
            // 1) Get current teacher’s ClassName
            var userMgr = HttpContext.GetOwinContext()
                                          .GetUserManager<ApplicationUserManager>();
            var appUser = userMgr.FindById(User.Identity.GetUserId());
            var teacherClass = appUser?.ClassName;

            // 2) Filter pending users to that class, order by newest first
            var list = db.PendingUsers
                         .Where(p => p.ClassName == teacherClass)
                         .OrderByDescending(p => p.Id)
                         .ToList();

            return View(list);
        }

        public ActionResult Approve(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var pu = db.PendingUsers.Find(id);
            if (pu == null)
                return HttpNotFound();

            // show the same details view but with Approve button
            return View("Details", pu);
        }

        // POST: PendingUsers/Approve/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ApproveConfirmed(int id)
        {
            var pu = db.PendingUsers.Find(id);
            if (pu == null)
                return HttpNotFound();

            // 0) Check for existing account
            var userMgr = HttpContext.GetOwinContext()
                                .GetUserManager<ApplicationUserManager>();
            var existing = await userMgr.FindByEmailAsync(pu.Email);
            if (existing != null)
            {
                // Inform the teacher and stay on Details view
                ModelState.AddModelError("",
                    $"A user with email {pu.Email} already exists.");
                return View("Details", pu);
            }

            // 1) Generate password and validate it
            var tempPwd = IdentityHelper.GenerateSecureTempPassword();

            // Check if password meets the criteria
            if (!IsValidPassword(tempPwd))
            {
                TempData["Fail"] = "Password generated does not meet the required criteria. Please try again.";
                return View("Details", pu);
            }

            // 2) Determine if this is a Student or Parent and create the respective object
            if (pu.RequestedRole == RequestedRol.Student)
            {
                // Create and save to ApprovedStudents table
                var student = new ApprovedStudent
                {
                    FirstName = pu.FirstName,
                    LastName = pu.LastName,
                    Email = pu.Email,
                    DateOfBirth = pu.DateOfBirth,
                    ClassName = pu.ClassName,
                    PhotoPath = pu.PhotoPath // Assuming this is the path from the PendingUser
                };

                // Add student to ApplicationDbContext (which has the ApprovedStudents table)
                db.ApprovedStudents.Add(student);
            }
            else if (pu.RequestedRole == RequestedRol.Parent)
            {
                // Create and save to ApprovedParents table
                var parent = new ApprovedParent
                {
                    FirstName = pu.FirstName,
                    LastName = pu.LastName,
                    Email = pu.Email,
                    PhotoPath = pu.PhotoPath
                };

                // Add parent to ApplicationDbContext (which has the ApprovedParents table)
                db.ApprovedParents.Add(parent);
            }

            // 3) Create Identity user
            var user = new ApplicationUser
            {
                UserName = pu.Email,
                Email = pu.Email,
                EmailConfirmed = true,
                ClassName = pu.ClassName // ClassName is only necessary if you want to assign it to the identity user
            };

            var createResult = await userMgr.CreateAsync(user, tempPwd);
            if (!createResult.Succeeded)
            {
                ModelState.AddModelError("", createResult.Errors.First());
                return View("Details", pu);
            }

            // 4) Assign the appropriate role (e.g., Student or Parent)
            await userMgr.AddToRoleAsync(user.Id, pu.RequestedRole.ToString());

            // 5) Send email with the credentials + reset link
            var token = await userMgr.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account",
                  new { userId = user.Id, code = token },
                  protocol: Request.Url.Scheme);

            var body = $@"
                Hello {pu.FirstName},<br/><br/>
                Your account has been approved!<br/>
                <b>Username:</b> {pu.Email}<br/>
                <b>Temporary password:</b> {tempPwd}<br/>
                Please <a href=""{callbackUrl}"">click here</a> to set your permanent password.
                ";

            await userMgr.EmailService.SendAsync(new IdentityMessage
            {
                Destination = pu.Email,
                Subject = "Your Higher Grades is Live",
                Body = body
            });

            // 6) Remove from pending users
            db.PendingUsers.Remove(pu);
            await db.SaveChangesAsync();

            // 7) Return to the approval waiting list
            TempData["Success"] = $"User {pu.Email} approved and invited.";
            return RedirectToAction("WaitingApproval");
        }

        // Password validation helper method
        private bool IsValidPassword(string password)
        {
            // Define password requirements
            var hasUpperCase = password.Any(c => char.IsUpper(c));
            var hasLowerCase = password.Any(c => char.IsLower(c));
            var hasDigit = password.Any(c => char.IsDigit(c));
            var hasSpecialChar = password.Any(c => "!@#$%^&*()".Contains(c));
            var isValidLength = password.Length >= 8;

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar && isValidLength;
        }

        // GET: PendingUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PendingUser pendingUser = db.PendingUsers.Find(id);
            if (pendingUser == null)
            {
                return HttpNotFound();
            }
            return View(pendingUser);
        }

        // GET: PendingUsers/Create
        public ActionResult Apply()
        {
            return View();
        }

        // POST: PendingUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Apply(
            [Bind(Include =
                "Email,FirstName,LastName,ClassName,DateOfBirth,RequestedRole")]
                PendingUser pendingUser,
                HttpPostedFileBase Photo)
        {
            // 1) Photo is required
            if (Photo == null || Photo.ContentLength == 0)
            {
                ModelState.AddModelError("Photo", "Please upload a profile photo.");
            }

            // 2) Now check the rest of the model
            if (!ModelState.IsValid)
                return View(pendingUser);

            // 3) Save the photo to disk
            var fileName = Path.GetFileName(Photo.FileName);
            var savedName = $"{Guid.NewGuid()}_{fileName}";
            var savePath = Path.Combine(Server.MapPath("~/Uploads"), savedName);
            Photo.SaveAs(savePath);
            pendingUser.PhotoPath = "/Uploads/" + savedName;

            // 4) Stamp the application time
            pendingUser.AppliedAt = DateTime.UtcNow;

            // 5) Persist and redirect home
            db.PendingUsers.Add(pendingUser);
            db.SaveChanges();

            TempData["Success"] = "Your application was successfully submitted!";
            return RedirectToAction("Index", "Home");

        }

        // GET: PendingUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PendingUser pendingUser = db.PendingUsers.Find(id);
            if (pendingUser == null)
            {
                return HttpNotFound();
            }
            return View(pendingUser);
        }

        // POST: PendingUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,FirstName,LastName,ClassName,DateOfBirth,PhotoPath,RequestedRole,AppliedAt")] PendingUser pendingUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pendingUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("WaitingApproval");
            }
            return View(pendingUser);
        }

        // GET: PendingUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PendingUser pendingUser = db.PendingUsers.Find(id);
            if (pendingUser == null)
            {
                return HttpNotFound();
            }
            return View(pendingUser);
        }

        // POST: PendingUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PendingUser pendingUser = db.PendingUsers.Find(id);
            db.PendingUsers.Remove(pendingUser);
            db.SaveChanges();
            return RedirectToAction("WaitingApproval");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
