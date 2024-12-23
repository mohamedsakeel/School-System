using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Domain.Entities
{
    public class Subject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SubjectName { get; set; } // Example: "Mathematics"
        public DateTime EnteredDate { get; set; }
        public bool IsElective { get; set; }
        public string EnteredBy { get; set; }
        public bool Status { get; set; }
    }
}
