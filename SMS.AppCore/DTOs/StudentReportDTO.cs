using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.DTOs
{
    public class StudentReportDTO
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public Dictionary<string, int?> Marks { get; set; } = new Dictionary<string, int?>();
        public int TotalMarks => Marks.Values.Where(m => m.HasValue).Sum(m => m ?? 0);
        public double AverageMarks => Marks.Values.Any(m => m.HasValue) ? Marks.Values.Where(m => m.HasValue).Average(m => m ?? 0) : 0;
        public int Rank { get; set; } // Will be assigned later
    }

}
