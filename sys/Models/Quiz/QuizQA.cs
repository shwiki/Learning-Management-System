using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sys.Models.Quiz
{
    public class QuizQA
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public int QuizId { get; set; }
        // ← navigation back to CreateQuiz
        [ForeignKey(nameof(QuizId))]
        public virtual CreateQ Quiz { get; set; }
        [Required]
        public string Topic { get; set; }

        [Required]
        [Display(Name = "Question")]
        public string QuestionText { get; set; }

        [Required]
        [Display(Name = "Option A")]
        public string OptionA { get; set; }

        [Required]
        [Display(Name = "Option B")]
        public string OptionB { get; set; }

        [Required]
        [Display(Name = "Option C")]
        public string OptionC { get; set; }

        [Required]
        [Display(Name = "Option D")]
        public string OptionD { get; set; }

        [Required]
        [Display(Name = "Answer")]
        [RegularExpression(@"[A-Da-d]", ErrorMessage = "Answer must be A, B, C, or D.")]
        public string Answer { get; set; }

        [Required]
        [Range(1, 100)]
        public int Marks { get; set; }

        [Required]
        public DifficultyLevel Difficulty { get; set; }
        public string Explanation { get; set; }

        public enum DifficultyLevel
        {
            Easy,
            Medium,
            Hard
        }
    }
}
