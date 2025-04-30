using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sys.Controllers.Dashboards
{
    public class StudentDashboardController : Controller
    {
        // GET: StudentDashboard
        public ActionResult Index()
        {
            return View();
        }
    }
}