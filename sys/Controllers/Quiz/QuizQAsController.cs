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
    public class QuizQAsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: QuizQAs?quizId=5
        public ActionResult Index(int? quizId)
        {
            var query = db.QuizQAs.Include(q => q.Quiz).AsQueryable();
            if (quizId.HasValue)
            {
                var quiz = db.CreateQs.Find(quizId.Value);
                if (quiz == null) return HttpNotFound();
                ViewBag.QuizTitle = quiz.QuizTitle;
                query = query.Where(q => q.QuizId == quizId.Value);
            }
            return View(query.ToList());
        }

        // GET: QuizQAs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizQA quizQA = db.QuizQAs.Find(id);
            if (quizQA == null)
            {
                return HttpNotFound();
            }
            return View(quizQA);
        }

        // GET: QuizQAs/Create?quizId=42
        public ActionResult Create(int quizId)
        {
            var quiz = db.CreateQs.Find(quizId);
            if (quiz == null) return HttpNotFound();

            ViewBag.QuizTitle = quiz.QuizTitle;
            ViewBag.Instructions = quiz.Instructions;

            // seed the FK property on the VM
            var vm = new QuizQA { QuizId = quizId };
            return View(vm);
        }


        // POST: QuizQAs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QuizId,Topic,QuestionText,OptionA,OptionB,OptionC,OptionD,Answer,Marks,Difficulty,Explanation")] QuizQA quizQA)
        {
            if (!ModelState.IsValid)
            {
                var quiz = db.CreateQs.Find(quizQA.QuizId);
                ViewBag.QuizTitle = quiz?.QuizTitle;
                ViewBag.Instructions = quiz?.Instructions;
                return View(quizQA);
            }

            db.QuizQAs.Add(quizQA);
            db.SaveChanges();
            return RedirectToAction("Create", new { quizId = quizQA.QuizId });
        }


        // GET: QuizQAs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizQA quizQA = db.QuizQAs.Find(id);
            if (quizQA == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuizId = new SelectList(db.CreateQs, "Id", "QuizTitle", quizQA.QuizId);
            return View(quizQA);
        }

        // POST: QuizQAs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,QuizId,Topic,QuestionText,OptionA,OptionB,OptionC,OptionD,Answer,Marks,Difficulty,Explanation")] QuizQA quizQA)
        {
            if (!ModelState.IsValid) return View(quizQA);
            db.Entry(quizQA).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { quizId = quizQA.QuizId });
        }

        // GET: QuizQAs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuizQA quizQA = db.QuizQAs.Find(id);
            if (quizQA == null)
            {
                return HttpNotFound();
            }
            return View(quizQA);
        }

        // POST: QuizQAs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var quizQA = db.QuizQAs.Find(id);
            int qid = quizQA.QuizId;
            db.QuizQAs.Remove(quizQA);
            db.SaveChanges();
            return RedirectToAction("Index", new { quizId = qid });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}