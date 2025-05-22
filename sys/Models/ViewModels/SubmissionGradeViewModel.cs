using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sys.Models.ViewModels
{
    public class SubmissionGradeViewModel
    {
        public int SubmissionId { get; set; }
        public string StudentName { get; set; }
        public DateTime SubmittedOn { get; set; }
        public string FilePath { get; set; }

        // If already graded, show existing
        public int? Mark { get; set; }
        public string Comments { get; set; }
    }
}