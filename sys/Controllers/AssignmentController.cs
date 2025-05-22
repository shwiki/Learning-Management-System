using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using sys.Models;
using sys.Models.Assignment;
using sys.Models.ViewModels;
using System.Data.Entity;

namespace sys.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Assignment/Details/{id}
        public ActionResult Details(int id)
        {
            var assignment = db.CreateAssignments.SingleOrDefault(a => a.Id == id);
            if (assignment == null)
                return HttpNotFound();

            return View(assignment);
        }

        // POST: Assignment/Upload/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(int id, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var fileName = System.IO.Path.GetFileName(file.FileName);
                var filePath = System.IO.Path.Combine(Server.MapPath("~/Uploads/Assignments"), fileName);
                file.SaveAs(filePath);

                // Save submission details to DB
                var userId = User.Identity.GetUserId();
                db.AssignmentSubmissions.Add(new AssignmentSubmission
                {
                    AssignmentId = id,
                    StudentId = userId,
                    FilePath = "/Uploads/Assignments/" + fileName,
                    SubmittedOn = DateTime.UtcNow
                });
                db.SaveChanges();

                return RedirectToAction("Details", new { id });
            }

            ModelState.AddModelError("", "Please select a file to upload.");
            var assignment = db.CreateAssignments.SingleOrDefault(a => a.Id == id);
            return View("Details", assignment);
        }
       
        public ActionResult Submissions(int assignmentId)
        {
            var assignment = db.CreateAssignments.Find(assignmentId);
            if (assignment == null) return HttpNotFound();

            // only ungraded submissions
            var subs = db.AssignmentSubmissions
                 .Include(s => s.Student)   // now resolved
                 .Include(s => s.Grades)    // now resolved
                 .Where(s => s.AssignmentId == assignmentId
                          && !s.Grades.Any())   // can check Grades
                 .OrderBy(s => s.SubmittedOn)
                 .ToList();

            var vm = new GradeAssignmentListViewModel
            {
                AssignmentId = assignmentId,
                AssignmentTitle = assignment.Title,
                Subject = assignment.Subject.ToString(),
                DueDate = assignment.EndDate,
                Submissions = subs.Select(s => new SubmissionGradeViewModel
                {
                    SubmissionId = s.Id,
                    StudentName = s.Student.UserName,
                    SubmittedOn = s.SubmittedOn,
                    FilePath = s.FilePath
                }).ToList()
            };
            return View(vm);
        }
        [Authorize(Roles = "Teacher")]
        public ActionResult Index()
        {
            // load all assignments for grading (e.g. by class, subject, or creator)
            var assignments = db.CreateAssignments
                                .OrderByDescending(a => a.StartDate)
                                .ToList();
            return View(assignments);
        }
        public ActionResult Grade(int submissionId)
        {
            var sub = db.AssignmentSubmissions
                        .Include(s => s.Student)
                        .FirstOrDefault(s => s.Id == submissionId);
            if (sub == null) return HttpNotFound();

            var grade = db.AssignmentGrades
                          .FirstOrDefault(g => g.SubmissionId == submissionId);

            var vm = new GradeSubmissionViewModel
            {
                SubmissionId = submissionId,
                AssignmentId = sub.AssignmentId,
                StudentName = sub.Student.UserName,
                SubmittedOn = sub.SubmittedOn,
                FilePath = sub.FilePath,
                Mark = grade?.Mark ?? 0,
                Comments = grade?.Comments
            };
            return View(vm);
        }
        [Authorize(Roles = "Teacher")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Grade(GradeSubmissionViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var existing = db.AssignmentGrades
                             .FirstOrDefault(g => g.SubmissionId == vm.SubmissionId);

            if (existing == null)
            {
                existing = new AssignmentGrade
                {
                    SubmissionId = vm.SubmissionId,
                    TeacherId = User.Identity.GetUserId()
                };
                db.AssignmentGrades.Add(existing);
            }

            existing.Mark = vm.Mark;
            existing.Comments = vm.Comments;
            existing.GradedOn = DateTime.UtcNow;

            db.SaveChanges();

            // after grading, redirect back to the submissions list
            return RedirectToAction("Submissions",
                                    new { assignmentId = vm.AssignmentId });
        }
    }
}
