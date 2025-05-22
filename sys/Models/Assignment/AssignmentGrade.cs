using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sys.Models.Assignment
{
    public class AssignmentGrade
    {
        public int Id { get; set; }

        // Link back to the student’s submission
        public int SubmissionId { get; set; }
        public virtual AssignmentSubmission Submission { get; set; }

        // The numeric score (e.g. out of 100)
        public int Mark { get; set; }

        // Teacher’s comments
        public string Comments { get; set; }

        // When it was graded
        public DateTime GradedOn { get; set; }

        // (Optional) Who graded it
        public string TeacherId { get; set; }
        public virtual ApplicationUser Teacher { get; set; }
    }
}