using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Domain.Entities
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }
        [Required]
        public string SubjectName { get; set; }
        public bool IsElective { get; set; } // Indicates if the subject is an elective
        public DateTime EnteredDate { get; set; }
        public bool Status { get; set; }
        public ICollection<Class> Classes { get; set; } = new List<Class>();
        public ICollection<SubjectAssignment> SubjectAssignments { get; set; } = new List<SubjectAssignment>();
        public ICollection<ElectiveGroup> ElectiveGroups { get; set; } = new List<ElectiveGroup>();
        public ICollection<SubjectExamStructure> SubjectExamStructures { get; set; } = new LinkedList<SubjectExamStructure>();
    }
}
