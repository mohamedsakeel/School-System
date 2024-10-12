using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Domain.Entities
{
    public class MCQAnswerSheet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnswerSheetId { get; set; }
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int SubjectId { get; set; } // Link to subject for which the MCQ is being graded
        [Required]
        public string SheetPath { get; set; } // Path to the uploaded answer sheet
        [Required]
        public int NumberOfQuestions { get; set; } // Number of questions in the MCQ paper

        [ForeignKey("StudentId")]
        public Student Student { get; set; }
        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }
    }
}
