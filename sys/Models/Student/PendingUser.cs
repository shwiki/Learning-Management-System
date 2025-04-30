using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using sys.Models;

namespace sys.Models.Student
{
    public class PendingUser
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public GradeNo ClassName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        // store the uploaded photo filename
        public string PhotoPath { get; set; }

        // Optional: Student vs Parent
        public RequestedRol RequestedRole { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public enum RequestedRol
        {
            Student,
            Parent
        }
        public enum GradeNo
        {
            Grade_1,
            Grade_2,
            Grade_3,
            ECD_A,
            ECD_B
        }
    }
}