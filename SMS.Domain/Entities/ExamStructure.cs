using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Domain.Entities
{
    public class ExamStructure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamStructureId { get; set; }
        [Required]
        public string ExamTemplateName { get; set; } // Template name for the exam structure

        public ICollection<Subject> Subjects { get; set; } = new List<Subject>(); // Subjects using this exam structure template
        public ICollection<ClassExamStructure> ClassExamStructures { get; set; } = new List<ClassExamStructure>();
        public ICollection<SubjectExamStructure> SubjectExamStructures { get; set; } = new List<SubjectExamStructure>();


    }
}

