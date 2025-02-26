using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.DTOs
{
    public class ClassSubjectMarksDTO
    {
        public string ClassName { get; set; }
        public int ClassId { get; set; }
        public List<SubjectDTO> SubjectsList { get; set; } = new List<SubjectDTO>();
        public List<string> Subjects { get; set; } = new List<string>();
        public List<StudentMarksDTO> Students { get; set; } = new List<StudentMarksDTO>();
        public string TeacherName { get; set; }
        public int CompletionPercentage { get; set; }
    }

    public class ClassSubjectMarksViewModel
    {
        public IEnumerable<ClassSubjectMarksDTO> ClassSubjectMarkss { get; set; } = new List<ClassSubjectMarksDTO>();
    }
}
