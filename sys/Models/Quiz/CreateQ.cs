using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static sys.Models.Student.PendingUser;

namespace sys.Models.Quiz
{
    public class CreateQ
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The title must be less than 100 characters.")]
        public string QuizTitle { get; set; }
        [Required]
        public SubjectSelect Subject { get; set; }

        [StringLength(500, ErrorMessage = "Instructions must be less than 500 characters.")]
        public string Instructions { get; set; }

        [Required]
        [DataType(DataType.DateTime, ErrorMessage = "Please enter a valid start date.")]
        public DateTime StartDate { get; set; }
        public GradeNo ClassName { get; set; }
        [Required]
        [DataType(DataType.DateTime, ErrorMessage = "Please enter a valid end date.")]
        public DateTime EndDate { get; set; }
        public virtual ICollection<QuizQA> Questions { get; set; }
        public bool PublishGradeAfterSubmission { get; set; }
        
        public enum SubjectSelect
        {
            Shona,
            Maths,
            FAREME,
            PE,
            English,
            ICT
        }

    }
}