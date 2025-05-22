using Microsoft.AspNet.Identity;
using sys.Models;
using sys.Models.Approval;
using sys.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static sys.Models.Student.PendingUser;

namespace sys.Controllers.Dashboards
{
    public class ParentDashboardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ParentDashboard
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Parent")]
        public ActionResult LinkChild()
        {
            var vm = new LinkChildViewModel
            {
                AvailableClasses = Enum.GetValues(typeof(GradeNo))
                    .Cast<GradeNo>()
                    .Select(g => new SelectListItem
                    {
                        Text = g.ToString(),
                        Value = ((int)g).ToString()
                    })
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Parent")]
        public async Task<ActionResult> LinkChild(LinkChildViewModel vm)
        {
            // repopulate the dropdown on error
            vm.AvailableClasses = Enum.GetValues(typeof(GradeNo))
                .Cast<GradeNo>()
                .Select(g => new SelectListItem
                {
                    Text = g.ToString(),
                    Value = ((int)g).ToString()
                });

            if (!ModelState.IsValid)
                return View(vm);

            // look up the student by email + class
            var student = await db.Users
                .FirstOrDefaultAsync(u => u.Email == vm.StudentEmail
                                       && u.ClassName == vm.ClassName);

            if (student == null)
            {
                ModelState.AddModelError(nameof(vm.StudentEmail),
                    "No student found with that email in the selected class.");
                return View(vm);
            }

            // create the link
            var parentId = User.Identity.GetUserId();    // no UserManager needed
            db.UserParentChild.Add(new UserParentChild
            {
                ParentId = parentId,
                ChildId = student.Id
            });
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}