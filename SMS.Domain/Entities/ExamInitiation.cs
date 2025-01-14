using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Domain.Entities
{
    public class ExamInitiation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ExamId { get; set; } // Foreign Key to Exam
        public bool IsInitiated { get; set; } = false; // Default to not initiated
        public string InitiatedBy { get; set; } // User who initiated
        public DateTime InitiatedDate { get; set; } = DateTime.Now; // Timestamp
    }
}
