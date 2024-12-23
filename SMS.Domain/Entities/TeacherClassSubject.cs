using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Domain.Entities
{
    public class TeacherClassSubject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TeacherId { get; set; } // Foreign Key to ApplicationUser (manual connection)
        public int ClassId { get; set; } // Foreign Key to Class (manual connection)
        public int SubjectId { get; set; } // Foreign Key to Subject (manual connection)
    }
}
