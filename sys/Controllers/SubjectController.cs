using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using sys.Models;
using System.Data.Entity;

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

            // 1) Current time check  
            var now = DateTime.UtcNow;

            // 2) Quizzes within date range & matching subject + class  
            var quizzes = db.CreateQs
                            .Where(q => q.Subject.ToString() == subject
                                     && q.ClassName == className
                                     && q.StartDate <= now
                                     && q.EndDate >= now)
                            .OrderByDescending(q => q.Id)
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
                Quizzes = quizzes,
                Assignments = assignments,
                Notes = notes,
                
            };

            return View(vm);
        }
    }
}
