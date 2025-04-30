using sys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sys.Controllers.Dashboards
{
    public class TeacherDashboardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: TeacherDashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MyClass()
        {
            return View(db.ApprovedStudents.ToList());
        }
    }
}