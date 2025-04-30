using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sys.Models.Student
{
    public class ReviewItemView
    {
        public int Number { get; set; }   // 1-based question number
        public int QuestionIndex { get; set; }   // zero-based index for navigation
        public int QuestionId { get; set; }
        public string Text { get; set; }   // question text
        public string SelectedOption { get; set; }   // "A","B","C","D"
        public string SelectedText { get; set; }   // e.g. "17" if OptionB was "17"
    }
}