using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sys.Models.Quiz
{
    public class AttemptAnswer
    {
      
            public int Id { get; set; }
            public int QuizAttemptId { get; set; }           // FK
            public int QuestionId { get; set; }           // FK to QuizQuestion
            public string Selected { get; set; }           // A/B/C/D
            public bool IsCorrect { get; set; }           // against correct Answer
            public virtual QuizAttempt QuizAtt { get; set; }
            public virtual QuizQA Question { get; set; }
        

    }
}