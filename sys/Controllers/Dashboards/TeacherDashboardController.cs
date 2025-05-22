using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
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