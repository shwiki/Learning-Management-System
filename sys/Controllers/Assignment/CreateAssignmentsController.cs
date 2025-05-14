using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using sys.Models;
using sys.Models.Assignment;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace sys.Controllers.Assignment
{
    public class CreateAssignmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CreateAssignments
        public ActionResult Index(int page = 1)
        {
            // 1) Get the current teacher’s ClassName
            var userMgr = HttpContext.GetOwinContext()
                                          .GetUserManager<ApplicationUserManager>();
            var appUser = userMgr.FindById(User.Identity.GetUserId());
            var teacherClass = appUser?.ClassName;

            // 2) Build filtered, sorted query
            const int pageSize = 4;
            var query = db.CreateAssignments
                          .Where(a => a.ClassName == teacherClass)
                          .OrderByDescending(a => a.Id);

            // 3) Fetch the page
            var paged = query
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

            // 4) Pass paging info to view
            ViewBag.Page = page;
            ViewBag.HasNext = query.Count() > page * pageSize;

            return View(paged);
        }

        // GET: CreateAssignments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CreateAssignment createAssignment = db.CreateAssignments.Find(id);
            if (createAssignment == null)
            {
                return HttpNotFound();
            }
            return View(createAssignment);
        }

        // GET: CreateAssignments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CreateAssignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
    [Bind(Include = "Id,Subject,ClassName,StartDate,EndDate,FilePath")]
    CreateAssignment createAssignment,
    HttpPostedFileBase FileUpload)
        {
            // 0) Auto-inject the teacher's ClassName from their Identity user
            var userId = User.Identity.GetUserId();
            var userMgr = HttpContext.GetOwinContext()
                                     .GetUserManager<ApplicationUserManager>();
            var appUser = userMgr.FindById(userId);
            // assume ApplicationUser has a ClassName property
            createAssignment.ClassName = (Models.Student.PendingUser.GradeNo)(appUser?.ClassName);

            // 1) Handle file upload
            if (FileUpload != null && FileUpload.ContentLength > 0)
            {
                var uploadsDir = Server.MapPath("~/Uploads");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var fileName = Path.GetFileName(FileUpload.FileName);
                var uniqueName = $"{Guid.NewGuid():N}_{fileName}";
                var fullPath = Path.Combine(uploadsDir, uniqueName);

                FileUpload.SaveAs(fullPath);
                createAssignment.FilePath = "/Uploads/" + uniqueName;
            }

            // 2) Save if valid
            if (ModelState.IsValid)
            {
                db.CreateAssignments.Add(createAssignment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // 3) On error, re-show form
            return View(createAssignment);
        }

        // GET: CreateAssignments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CreateAssignment createAssignment = db.CreateAssignments.Find(id);
            if (createAssignment == null)
            {
                return HttpNotFound();
            }
            return View(createAssignment);
        }

        // POST: CreateAssignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Subject, ClassName,StartDate,EndDate,FilePath")] CreateAssignment createAssignment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(createAssignment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(createAssignment);
        }

        // GET: CreateAssignments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CreateAssignment createAssignment = db.CreateAssignments.Find(id);
            if (createAssignment == null)
            {
                return HttpNotFound();
            }
            return View(createAssignment);
        }

        // POST: CreateAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CreateAssignment createAssignment = db.CreateAssignments.Find(id);
            db.CreateAssignments.Remove(createAssignment);
            db.SaveChanges();
            return RedirectToAction("Index");
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
