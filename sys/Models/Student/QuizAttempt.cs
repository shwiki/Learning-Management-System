using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sys.Models.Quiz
{
    public class QuizAttempt
    {
            public int Id { get; set; }
            public string StudentId { get; set; }           // FK to AspNetUsers
            public int QuizId { get; set; }           // FK to CreateQuiz
            public DateTime Started { get; set; }
            public DateTime? Completed { get; set; }
            public int Score { get; set; }           // computed at the end
            public virtual CreateQ Quiz { get; set; }
            public virtual ApplicationUser Student { get; set; }
            public virtual ICollection<AttemptAnswer> Answers { get; set; }
    }
}