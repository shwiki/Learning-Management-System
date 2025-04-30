using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;         // ← for .Include()
using sys.Models.Quiz;
using sys.Models.Student;
using sys;
using sys.Models;                        // ← for ApplicationDbContext

namespace sys.Controllers.Student
{
    public class QuizAttemptController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Quiz/Available
        public ActionResult Available()
        {
            var now = DateTime.Now;
            var availableQuizzes = db.CreateQs
                                     .Where(q => q.StartDate <= now && q.EndDate >= now)
                                     .OrderBy(q => q.StartDate)
                                     .ToList();
            return View(availableQuizzes);
        }

        // GET: /Quiz/Confirm?quizId=5
        public ActionResult Confirm(int quizId)
        {
            var quiz = db.CreateQs.Find(quizId);
            if (quiz == null) return HttpNotFound();
            return View(quiz);
        }

        // POST: /Quiz/Start
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Start(int quizId)
        {
            var attempt = new QuizAttempt
            {
                QuizId = quizId,
                StudentId = User.Identity.GetUserId(),
                Started = DateTime.Now
            };
            db.QuizAttempts.Add(attempt);
            db.SaveChanges();

            return RedirectToAction(
                actionName: "Take",
                routeValues: new { attemptId = attempt.Id }
            );
        }

        // GET: /QuizAttempt/Take?attemptId=7&index=0
        [HttpGet]
        public ActionResult Take(int attemptId, int index = 0)
        {
            var attempt = db.QuizAttempts
                            .Include(a => a.Quiz.Questions)
                            .SingleOrDefault(a => a.Id == attemptId);
            if (attempt == null) return HttpNotFound();

            var questions = attempt.Quiz.Questions
                .OrderBy(x => x.Id)
                .ToList();

            if (index < 0 || index >= questions.Count)
                return RedirectToAction("Finish", new { attemptId });

            var question = questions[index];

            var existingAnswer = db.AttemptAnswers
                                   .SingleOrDefault(a =>
                                       a.QuizAttemptId == attemptId &&
                                       a.QuestionId == question.Id);

            var vm = new TakeQuestionView
            {
                AttemptId = attemptId,
                QuestionIndex = index,
                QuestionId = question.Id,
                QuestionText = question.QuestionText,
                OptionA = question.OptionA,
                OptionB = question.OptionB,
                OptionC = question.OptionC,
                OptionD = question.OptionD,
                SelectedOption = existingAnswer?.Selected,
                IsLast = index == questions.Count - 1
            };

            // explicitly reference your "TakeQuestionView.cshtml"
            return View("TakeQuestionView", vm);
        }

        // POST: /QuizAttempt/Take
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Take(TakeQuestionView vm, string nav)
        {
            // 1) Validation: send them back to the same question if they forgot to pick an answer
            if (!ModelState.IsValid)
                return RedirectToAction("Take", new
                {
                    attemptId = vm.AttemptId,
                    index = vm.QuestionIndex
                });

            // 2) Save or update their answer
            var answer = db.AttemptAnswers
                           .SingleOrDefault(a =>
                               a.QuizAttemptId == vm.AttemptId &&
                               a.QuestionId == vm.QuestionId);

            if (answer == null)
            {
                answer = new AttemptAnswer
                {
                    QuizAttemptId = vm.AttemptId,
                    QuestionId = vm.QuestionId
                };
                db.AttemptAnswers.Add(answer);
            }
            answer.Selected = vm.SelectedOption;
            db.SaveChanges();

            // 3) Navigation logic
            if (nav == "Back" && vm.QuestionIndex > 0)
            {
                return RedirectToAction("Take", new
                {
                    attemptId = vm.AttemptId,
                    index = vm.QuestionIndex - 1
                });
            }

            if (nav == "Next" && !vm.IsLast)
            {
                return RedirectToAction("Take", new
                {
                    attemptId = vm.AttemptId,
                    index = vm.QuestionIndex + 1
                });
            }

            // New: review step
            if (nav == "Review" && vm.IsLast)
            {
                return RedirectToAction("Review", new
                {
                    attemptId = vm.AttemptId
                });
            }

            // Optional: direct submit from last question
            if (nav == "Submit" && vm.IsLast)
            {
                return RedirectToAction("Finish", new
                {
                    attemptId = vm.AttemptId
                });
            }

            // Fallback: reload this question
            return RedirectToAction("Take", new
            {
                attemptId = vm.AttemptId,
                index = vm.QuestionIndex
            });
        }

        // GET: /QuizAttempt/Finish?attemptId=7
        public ActionResult Finish(int attemptId)
        {
            var attempt = db.QuizAttempts
                            .Include(a => a.Quiz.Questions)
                            .Include(a => a.Answers)
                            .SingleOrDefault(a => a.Id == attemptId);
            if (attempt == null) return HttpNotFound();

            if (attempt.Completed == null)
            {
                int earned = 0;
                foreach (var q in attempt.Quiz.Questions)
                {
                    var ans = attempt.Answers.SingleOrDefault(a => a.QuestionId == q.Id);
                    if (ans != null &&
                        ans.Selected.Equals(q.Answer, StringComparison.OrdinalIgnoreCase))
                    {
                        ans.IsCorrect = true;
                        earned += q.Marks;
                    }
                }
                attempt.Score = earned;
                attempt.Completed = DateTime.Now;
                db.SaveChanges();
            }

            return RedirectToAction("Result", new { attemptId });
        }
        // GET: /QuizAttempt/Review?attemptId=7
        // GET: /QuizAttempt/Review?attemptId=7
        public ActionResult Review(int attemptId)
        {
            var attempt = db.QuizAttempts
                            .Include(a => a.Quiz.Questions)
                            .Include(a => a.Answers)
                            .SingleOrDefault(a => a.Id == attemptId);
            if (attempt == null) return HttpNotFound();

            // order your questions once, so index lines up
            var questions = attempt.Quiz.Questions
                                 .OrderBy(q => q.Id)
                                 .ToList();

            var reviewItems = questions
                .Select((q, idx) => {
                    // find their answer record
                    var ans = attempt.Answers
                                     .FirstOrDefault(a => a.QuestionId == q.Id);

                    // map "A"->q.OptionA, etc.
                    string selText = null;
                    if (ans?.Selected == "A") selText = q.OptionA;
                    else if (ans?.Selected == "B") selText = q.OptionB;
                    else if (ans?.Selected == "C") selText = q.OptionC;
                    else if (ans?.Selected == "D") selText = q.OptionD;
                    else selText = "<no answer>";

                    return new ReviewItemView
                    {
                        Number = idx + 1,
                        QuestionIndex = idx,
                        QuestionId = q.Id,
                        Text = q.QuestionText,
                        SelectedOption = ans?.Selected,
                        SelectedText = selText
                    };
                })
                .ToList();

            ViewBag.AttemptId = attemptId;
            ViewBag.QuizTitle = attempt.Quiz.QuizTitle;
            return View(reviewItems);
        }


        // GET: /QuizAttempt/Result?attemptId=7
        public ActionResult Result(int attemptId)
        {
            var attempt = db.QuizAttempts
                            .Include(a => a.Quiz)
                            .SingleOrDefault(a => a.Id == attemptId);
            if (attempt == null) return HttpNotFound();
            return View(attempt);
        }
    }
}
