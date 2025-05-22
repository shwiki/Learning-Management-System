using sys.Models.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sys.Models.ViewModels
{
    public class AssignmentDetailsViewModel
    {
        public CreateAssignment Assignment { get; set; }
        public AssignmentSubmission Submission { get; set; }
    }
}