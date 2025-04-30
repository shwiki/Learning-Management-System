using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static sys.Models.Student.PendingUser;

namespace sys.Models
{
    public class CreateTeacherViewModel
    {
        [Key]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [Required]
        [Display(Name = "Class Name")]
        public GradeNo ClassName { get; set; }
    }
}