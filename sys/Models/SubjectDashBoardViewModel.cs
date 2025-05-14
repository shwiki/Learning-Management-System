using sys.Models.Assignment;
using sys.Models.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sys.Models
{
    public class SubjectDashBoardViewModel
    {
        public string SubjectName { get; set; }

        public IEnumerable<CreateQ> Quizzes { get; set; }
        public IEnumerable<CreateAssignment> Assignments { get; set; }
        public IEnumerable<Notes> Notes { get; set; }
        public IEnumerable<QuizAttempt> ExamResults { get; set; }
    }
}