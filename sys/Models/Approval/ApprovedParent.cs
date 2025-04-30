using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sys.Models.Approval
{
    public class ApprovedParent
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhotoPath { get; set; }
    }
}