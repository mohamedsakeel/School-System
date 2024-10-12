using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Domain.Entities
{
    public class ElectiveGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ElectiveGroupId { get; set; }
        [Required]
        public string GroupName { get; set; } // E.g., "Grade 6 Electives"
        public DateTime EnteredDate { get; set; }
        public bool Status { get; set; }

        public ICollection<Subject> ElectiveSubjects { get; set; } = new List<Subject>();
    }
}
