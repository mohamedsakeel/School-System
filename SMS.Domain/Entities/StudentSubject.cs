using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Domain.Entities
{
    public class StudentSubject
    {
        [Key]
        public int StudentSubjectId { get; set; }
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int ElectiveSubjectId { get; set; } // Only for elective subjects

        [ForeignKey("StudentId")]
        public Student Student { get; set; }
        [ForeignKey("ElectiveSubjectId")]
        public Subject Subject { get; set; }
    }
}
