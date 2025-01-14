using SMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.DTOs
{
    public class TeacherDTO
    {
        public int Id { get; set; }
        public string TeacherId { get; set; } 
        public int ClassId { get; set; } 
        public int SubjectId { get; set; }
        public string FullName { get; set; }
        public string Classs { get; set; }
        public string Subject { get; set; }

    }

    public class TeacherViewModel
    {
        public TeacherDTO TeacherClass { get; set; }
        public IEnumerable<TeacherDTO> TeachersAssinged { get; set; } = new List<TeacherDTO>();
        public IEnumerable<UserDTO> Teachers { get; set; } = new List<UserDTO>();
        public IEnumerable<ClassDTO> Classes { get; set; } = new List<ClassDTO>();
        public IEnumerable<SubjectDTO> Subjects { get; set; } = new List<SubjectDTO>();
        //list of strings to hold classess and subjects
        public List<string> SelectedClasses { get; set; } = new List<string>();
        public List<string> SelectedSubjects { get; set; } = new List<string>();
        public string SelectedTeacher { get; set; }
        public IEnumerable<TeacherAssignmentDTO> TeacherAssignments { get; set; } = new List<TeacherAssignmentDTO>();
    }

    public class TeacherAssignmentDTO
    {
        public string TeacherName { get; set; }
        public string TeacherId { get; set; }
        public IEnumerable<string> AssignedClasses { get; set; }
        public IEnumerable<string> AssignedSubjects { get; set; }
    }
}
