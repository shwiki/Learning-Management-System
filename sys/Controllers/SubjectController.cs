using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using sys.Models;
using System.Data.Entity;
using sys.Models.Assignment;
using System.IO;
using sys.Models.ViewModels;

namespace sys.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Subject/Dashboard?subject=Mathematics  
        public ActionResult Dashboard(string subject)
        {
            // 0) Get current user and their class  
            var userId = User.Identity.GetUserId();
            var userMgr = HttpContext.GetOwinContext()
                                       .GetUserManager<ApplicationUserManager>();
            var appUser = userMgr.FindById(userId);
            var className = appUser?.ClassName;
            var now = DateTime.UtcNow;

            // 1) quizzes currently open for *this* subject+class
            var available = db.CreateQs
                            .Where(q =>
                                q.Subject.ToString() == subject &&
                                q.ClassName == className &&
                                q.StartDate <= now &&
                                q.EndDate >= now &&
                                // ← same exclusion here
                                !db.QuizAttempts.Any(qa =>
                                    qa.QuizId == q.Id &&
                                    qa.StudentId == userId))
                            .OrderBy(q => q.StartDate)
                            .ToList();
            // 2) the student’s past attempts in this subject
            var attempts = db.QuizAttempts
                             .Include(qa => qa.Quiz)  // if you have a nav-prop `public CreateQ Quiz {get;set;}`
                             .Where(qa =>
                                 qa.StudentId == userId &&
                                 qa.Quiz.Subject.ToString() == subject)
                             .OrderByDescending(qa => qa.Started)
                             .ToList();


            // 3) Assignments within date range  
            var assignments = db.CreateAssignments
                                .Where(a => a.Subject.ToString() == subject
                                         && a.ClassName == className
                                         && a.StartDate <= now
                                         && a.EndDate >= now)
                                .OrderByDescending(a => a.Id)
                                .ToList();

            // 4) Notes (no date filter)  
            var notes = db.Notes
                          .Where(n => n.Subject.ToString() == subject
                                   && n.ClassName == className)
                          .OrderByDescending(n => n.Id)
                          .ToList();

            // 6) Populate ViewModel  
            var vm = new SubjectDashBoardViewModel
            {
                SubjectName = subject,
                AvailableQuizzes = available,
                ExamResults = attempts,
                Assignments = assignments,
                Notes = notes

            };

            return View(vm);
        }
        // GET: Subject/AssignmentDetails/5
        public ActionResult AssignmentDetails(int id)
        {
            var assignment = db.CreateAssignments
                               .FirstOrDefault(a => a.Id == id);
            if (assignment == null)
                return HttpNotFound();

            var studentId = User.Identity.GetUserId();
            var existing = db.AssignmentSubmissions
                             .FirstOrDefault(s => s.AssignmentId == id
                                               && s.StudentId == studentId);

            var vm = new AssignmentDetailsViewModel
            {
                Assignment = assignment,
                Submission = existing
            };
            return View(vm);
        }

        // POST: Subject/SubmitAssignment/5
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SubmitAssignment(int id, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var unique = $"{Guid.NewGuid()}_{fileName}";
                var folder = Server.MapPath("~/Uploads/Assignments");
                Directory.CreateDirectory(folder);
                var fullPath = Path.Combine(folder, unique);
                file.SaveAs(fullPath);

                var submission = new AssignmentSubmission
                {
                    AssignmentId = id,
                    StudentId = User.Identity.GetUserId(),
                    FilePath = "/Uploads/Assignments/" + unique,
                    SubmittedOn = DateTime.UtcNow
                };
                db.AssignmentSubmissions.Add(submission);
                db.SaveChanges();
            }
            return RedirectToAction("AssignmentDetails", new { id });
        }
    }
}
