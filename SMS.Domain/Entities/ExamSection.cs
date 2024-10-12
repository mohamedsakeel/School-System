using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Domain.Entities
{
    public class ExamSection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamSectionId { get; set; }
        [Required]
        public int ExamStructureId { get; set; }
        [Required]
        public string SectionName { get; set; } // Part 1, Part 2, MCQ
        [Required]
        public double MarksObtained { get; set; } // Total marks obtained in the section

        [ForeignKey("ExamStructureId")]
        public ExamStructure ExamStructure { get; set; }
        public DateTime EnteredDate { get; set; }
        public bool Status { get; set; }
    }
}
