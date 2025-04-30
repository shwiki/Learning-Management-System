using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static sys.Models.Student.PendingUser;

namespace sys.Models.Approval
{
    public class ApprovedStudent
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public GradeNo ClassName { get; set; }
        public string PhotoPath { get; set; }
    }
}