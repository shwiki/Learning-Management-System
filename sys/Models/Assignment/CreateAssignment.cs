using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static sys.Models.Quiz.CreateQ;
using static sys.Models.Student.PendingUser;

namespace sys.Models.Assignment
{
    public class CreateAssignment
    {
        public int Id { get; set; }
        [Required]
        public SubjectSelect Subject { get; set; }
        public string Title { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public GradeNo ClassName { get; set; }
        public string FilePath { get; set; }
        [NotMapped]
        [Display(Name = "Upload File")]
        public IFormFile FileUpload { get; set; }

    }
}