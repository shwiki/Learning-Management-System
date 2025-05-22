using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace sys.Models.Assignment
{
    public class AssignmentSubmission
    {
        public AssignmentSubmission()
        {
            Grades = new HashSet<AssignmentGrade>();
        }

        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public string StudentId { get; set; }
        public string FilePath { get; set; }
        public DateTime SubmittedOn { get; set; }

        [ForeignKey("AssignmentId")]
        public virtual CreateAssignment Assignment { get; set; }

        [ForeignKey("StudentId")]
        public virtual ApplicationUser Student { get; set; }

        // ← NEW: navigation to all grades for this submission
        public virtual ICollection<AssignmentGrade> Grades { get; set; }
    }
}
