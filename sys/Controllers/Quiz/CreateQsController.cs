using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
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
            return View(db.CreateQs.ToList());
        }

        // GET: CreateQs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var createQ = db.CreateQs.Find(id);
            if (createQ == null) return HttpNotFound();
            return View(createQ);
        }

        // GET: CreateQs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CreateQs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QuizTitle,Subject,Instructions,StartDate,EndDate,PublishGradeAfterSubmission")] CreateQ createQ)
        {
            if (!ModelState.IsValid)
                return View(createQ);

            db.CreateQs.Add(createQ);
            db.SaveChanges();

            // <-- redirect straight into Add‑Question, carrying the new QuizId
            return RedirectToAction(
                actionName: "Create",
                controllerName: "QuizQAs",
                routeValues: new { quizId = createQ.Id }
            );
        }


        // GET: CreateQs/Edit/5
        public ActionResult Edit(int? id) 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CreateQ createQ = db.CreateQs.Find(id);
            if (createQ == null)
            {
                return HttpNotFound();
            }
            return View(createQ);
        }

        // POST: CreateQs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,QuizTitle,Subject,Instructions,StartDate,EndDate,PublishGradeAfterSubmission")] CreateQ createQ)
        {
            if (!ModelState.IsValid) return View(createQ);
            db.Entry(createQ).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: CreateQs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CreateQ createQ = db.CreateQs.Find(id);
            if (createQ == null)
            {
                return HttpNotFound();
            }
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

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }

}
