using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SMS.Domain.Enumeration;

namespace SMS.Domain.Entities
{
    public class ExamMarks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamMarksId { get; set; }
        [Required]
        public int StudentId { get; set; }
        public int? ExamSectionId { get; set; } // Nullable to allow overall subject marks
        public double? SubjectTotalMarks { get; set; } // Total marks for the subject (if applicable)
        [Required]
        public int ExamYear { get; set; } // Year of the exam
        [Required]
        public Term Term { get; set; } // e.g., 1st Term, 2nd Term, 3rd Term

        [ForeignKey("StudentId")]
        public Student Student { get; set; }
        [ForeignKey("ExamSectionId")]
        public ExamSection ExamSection { get; set; }
    }
}
