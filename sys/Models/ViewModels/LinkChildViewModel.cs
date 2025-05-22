using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static sys.Models.Student.PendingUser;

namespace sys.Models.ViewModels
{
    public class LinkChildViewModel
    {
        [Display(Name = "Student First Name")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; }

        [Display(Name = "Student Last Name")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Student Email")]
        public string StudentEmail { get; set; }

        [Required]
        [Display(Name = "Class / Grade")]
        public GradeNo ClassName { get; set; }
        public IEnumerable<SelectListItem> AvailableClasses { get; internal set; }
    }
}