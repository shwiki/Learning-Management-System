using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sys.Models.ViewModels
{
    public class GradeAssignmentListViewModel
    {
        public int AssignmentId { get; set; }
        public string AssignmentTitle { get; set; }
        public string Subject { get; set; }
        public DateTime DueDate { get; set; }
        public List<SubmissionGradeViewModel> Submissions { get; set; }
    }
}