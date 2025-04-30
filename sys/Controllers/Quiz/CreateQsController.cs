using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using sys.Models;
using sys.Models.Quiz;

namespace sys.Controllers.Quiz
{
    public class CreateQsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CreateQs
        public ActionResult Index()
        {
            // 1) Get the current teacher’s class from Identity
            var userId = User.Identity.GetUserId();
            var userMgr = HttpContext.GetOwinContext()
                                     .GetUserManager<ApplicationUserManager>();
            var appUser = userMgr.FindById(userId);
            var teacherClass = appUser?.ClassName;

            // 2) Pull only quizzes for that class, in ascending order
            var quizzes = db.CreateQs
                            .Where(q => q.ClassName == teacherClass)
                            .OrderBy(q => q.Id)
                            .ToList();

            // 3) Pass through any flash message
            ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View(quizzes);
        }

        // GET: CreateQs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var createQ = db.CreateQs.Find(id);
            if (createQ == null)
                return HttpNotFound();

            return View(createQ);
        }

        // GET: CreateQs/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
        [Bind(Include = "QuizTitle,Subject,Instructions,StartDate,EndDate,PublishGradeAfterSubmission")]
         CreateQ createQ)
        {
            // 0) Fill in ClassName from the current user
            var userId = User.Identity.GetUserId();
            var userMgr = HttpContext.GetOwinContext()
                                     .GetUserManager<ApplicationUserManager>();
            var appUser = userMgr.FindById(userId);
            createQ.ClassName = (Models.Student.PendingUser.GradeNo)(appUser?.ClassName);

            // 1) Validate model
            if (!ModelState.IsValid)
                return View(createQ);

            // 2) Persist
            db.CreateQs.Add(createQ);
            db.SaveChanges();

            // 3) Redirect to question builder
            return RedirectToAction(
                "Create",
                "QuizQAs",
                new { quizId = createQ.Id }
            );
        }

        // GET: CreateQs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var createQ = db.CreateQs.Find(id);
            if (createQ == null)
                return HttpNotFound();

            return View(createQ);
        }

        // POST: CreateQs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,QuizTitle,Subject,Instructions,StartDate,EndDate,PublishGradeAfterSubmission")] CreateQ createQ)
        {
            if (!ModelState.IsValid)
                return View(createQ);

            db.Entry(createQ).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: CreateQs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var createQ = db.CreateQs.Find(id);
            if (createQ == null)
                return HttpNotFound();

            return View(createQ);
        }

        // POST: CreateQs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var createQ = db.CreateQs.Find(id);
            db.CreateQs.Remove(createQ);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // NEW: GET CreateQs/SubmitQuiz
        // Called by your “Submit Quiz” link to set a success message
        public ActionResult SubmitQuiz()
        {
            TempData["SuccessMessage"] = "Quiz submitted successfully!";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
