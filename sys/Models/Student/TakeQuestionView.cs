using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sys.Models.Student
{
    public class TakeQuestionView
    {
        public int AttemptId { get; set; }
        public int QuestionIndex { get; set; }      // zero-based
        public int QuestionId { get; set; }

        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }

        [Required]
        public string SelectedOption { get; set; }     // “A”, “B”, “C” or “D”

        public bool IsFirst => QuestionIndex == 0;
        public bool IsLast { get; set; }      // set in controller


    }
}