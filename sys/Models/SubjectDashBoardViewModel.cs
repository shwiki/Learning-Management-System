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

        // quizzes the student *can* still start
        public IEnumerable<CreateQ> AvailableQuizzes { get; set; }

        // the student’s past attempts in this subject
        public IEnumerable<QuizAttempt> ExamResults { get; set; }

        public IEnumerable<CreateAssignment> Assignments { get; set; }
        public IEnumerable<Notes> Notes { get; set; }
    }
}