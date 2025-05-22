using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sys.Models.ViewModels
{
    public class GradeSubmissionViewModel
    {
        [HiddenInput]
        public int SubmissionId { get; set; }

        [HiddenInput]
        public int AssignmentId { get; set; }

        public string StudentName { get; set; }
        public DateTime SubmittedOn { get; set; }
        public string FilePath { get; set; }

        [Required, Range(0, 100)]
        public int Mark { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }
    }
}