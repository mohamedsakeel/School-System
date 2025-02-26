using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.DTOs
{
    public class ExamInitiationDTO
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public int Year { get; set; }
        public DateTime InitiatedDate { get; set; }
        public string InitiatedBy { get; set; }
        public bool IsActive { get; set; }
    }

    public class InititateExamViewModel
    {
        public ExamInitiationDTO ExamInitiate { get; set; }
        public IEnumerable<ExamDTO> Exams { get; set; } = new List<ExamDTO>();
    }
}
