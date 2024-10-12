using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Domain.Entities
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }
        public int IndexNo { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public int ClassId { get; set; } // Foreign key to Class

        [ForeignKey("ClassId")]
        public Class Class { get; set; }
        public DateTime EnteredDate { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<ExamMarks> ExamMarks { get; set; } = new List<ExamMarks>();
    }
}
