using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using sys.Models;

namespace sys.Controllers
{
    public class NotesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Notes
        public ActionResult Index(int page = 1)
        {
            // 1) Get current teacher’s ClassName from Identity
            var userMgr = HttpContext.GetOwinContext()
                                       .GetUserManager<ApplicationUserManager>();
            var appUser = userMgr.FindById(User.Identity.GetUserId());
            var teacherClass = appUser?.ClassName;

            // 2) Build the query (filtered + sorted)
            const int pageSize = 4;
            var notesQuery = db.Notes
                               .Where(n => n.ClassName == teacherClass)
                               .OrderByDescending(n => n.Id);

            // 3) Execute paging
            var pagedNotes = notesQuery
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            // 4) Pass paging info to the view
            ViewBag.Page = page;
            ViewBag.HasNext = notesQuery.Count() > page * pageSize;

            return View(pagedNotes);
        }

        // GET: Notes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notes notes = db.Notes.Find(id);
            if (notes == null)
            {
                return HttpNotFound();
            }
            return View(notes);
        }

        // GET: Notes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Notes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
    [Bind(Include = "Id,Topic,KeyContent,Description,Image,Subject")] Notes notes,
    HttpPostedFileBase imageFile)
        {
            // 0) Auto‐fill ClassName from the current teacher’s profile
            var userId = User.Identity.GetUserId();
            var userMgr = HttpContext.GetOwinContext()
                                     .GetUserManager<ApplicationUserManager>();
            var appUser = userMgr.FindById(userId);
            notes.ClassName = (Models.Student.PendingUser.GradeNo)(appUser?.ClassName);

            // 1) File upload
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                var uploadsDir = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var fileName = Guid.NewGuid().ToString("N") + "_" + Path.GetFileName(imageFile.FileName);
                var fullPath = Path.Combine(uploadsDir, fileName);
                imageFile.SaveAs(fullPath);

                notes.Image = "/Uploads/" + fileName;
            }

            // 2) Persist
            if (ModelState.IsValid)
            {
                db.Notes.Add(notes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // 3) Redisplay if validation fails
            return View(notes);
        }
        // GET: Notes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notes notes = db.Notes.Find(id);
            if (notes == null)
            {
                return HttpNotFound();
            }
            return View(notes);
        }

        // POST: Notes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Level,Topic,KeyContent,Description,Image,Subject")] Notes notes, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Check if a new image file has been uploaded
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    string filePath = Path.Combine(folderPath, fileName);
                    imageFile.SaveAs(filePath);
                    notes.Image = "/Uploads/" + fileName;
                }

                db.Entry(notes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(notes);
        }

        // GET: Notes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notes notes = db.Notes.Find(id);
            if (notes == null)
            {
                return HttpNotFound();
            }
            return View(notes);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Notes notes = db.Notes.Find(id);
            db.Notes.Remove(notes);
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
