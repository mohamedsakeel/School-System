using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.DTOs
{
    public class StudentMarksDTO
    {
        public int Id { get; set; }
        public int IndexNo { get; set; }
        public string FullName { get; set; }
        public Dictionary<int, int> Marks { get; set; } = new Dictionary<int, int>(); // SubjectId -> Marks
    }

}
