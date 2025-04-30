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

namespace sys.Controllers.Student
{
    public class AttemptAnswersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AttemptAnswers
        public ActionResult Index()
        {
            var attemptAnswers = db.AttemptAnswers.Include(a => a.Question).Include(a => a.QuizAtt);
            return View(attemptAnswers.ToList());
        }

        // GET: AttemptAnswers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttemptAnswer attemptAnswer = db.AttemptAnswers.Find(id);
            if (attemptAnswer == null)
            {
                return HttpNotFound();
            }
            return View(attemptAnswer);
        }

        // GET: AttemptAnswers/Create
        public ActionResult Create()
        {
            ViewBag.QuestionId = new SelectList(db.QuizQAs, "Id", "Topic");
            ViewBag.QuizAttemptId = new SelectList(db.QuizAttempts, "Id", "StudentId");
            return View();
        }

        // POST: AttemptAnswers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,QuizAttemptId,QuestionId,Selected,IsCorrect")] AttemptAnswer attemptAnswer)
        {
            if (ModelState.IsValid)
            {
                db.AttemptAnswers.Add(attemptAnswer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.QuestionId = new SelectList(db.QuizQAs, "Id", "Topic", attemptAnswer.QuestionId);
            ViewBag.QuizAttemptId = new SelectList(db.QuizAttempts, "Id", "StudentId", attemptAnswer.QuizAttemptId);
            return View(attemptAnswer);
        }

        // GET: AttemptAnswers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttemptAnswer attemptAnswer = db.AttemptAnswers.Find(id);
            if (attemptAnswer == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuestionId = new SelectList(db.QuizQAs, "Id", "Topic", attemptAnswer.QuestionId);
            ViewBag.QuizAttemptId = new SelectList(db.QuizAttempts, "Id", "StudentId", attemptAnswer.QuizAttemptId);
            return View(attemptAnswer);
        }

        // POST: AttemptAnswers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,QuizAttemptId,QuestionId,Selected,IsCorrect")] AttemptAnswer attemptAnswer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attemptAnswer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.QuestionId = new SelectList(db.QuizQAs, "Id", "Topic", attemptAnswer.QuestionId);
            ViewBag.QuizAttemptId = new SelectList(db.QuizAttempts, "Id", "StudentId", attemptAnswer.QuizAttemptId);
            return View(attemptAnswer);
        }

        // GET: AttemptAnswers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttemptAnswer attemptAnswer = db.AttemptAnswers.Find(id);
            if (attemptAnswer == null)
            {
                return HttpNotFound();
            }
            return View(attemptAnswer);
        }

        // POST: AttemptAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AttemptAnswer attemptAnswer = db.AttemptAnswers.Find(id);
            db.AttemptAnswers.Remove(attemptAnswer);
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
