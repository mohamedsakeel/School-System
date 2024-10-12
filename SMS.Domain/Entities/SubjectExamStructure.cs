using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Domain.Entities
{
    public class SubjectExamStructure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubjectExamStructureId { get; set; }
        [Required]
        public int SubjectId { get; set; }
        [Required]
        public int ExamStructureId { get; set; }

        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; }
        [ForeignKey("ExamStructureId")]
        public ExamStructure ExamStructure { get; set; }
        public DateTime EnteredDate { get; set; }
        public bool Status { get; set; }
    }
}
