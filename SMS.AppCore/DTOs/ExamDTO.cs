using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.DTOs
{
    public class ExamDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } // Example: "First Term"
        public int Year { get; set; }
        public DateTime EnteredDate { get; set; }
        public string EnteredBy { get; set; }
        public bool Status { get; set; }
        public string StatusText
        {
            get
            {
                return Status ? "Active" : "Inactive";
            }
        }
    }

    public class ExamViewModel
    {
        public ExamDTO Exam { get; set; }
        public IEnumerable<ExamDTO> Exams { get; set; } = new List<ExamDTO>();
    }
}
