using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.DTOs
{
    public class StudentDTO
    {
        public int Id { get; set; }
        public int IndexNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public DateTime EnteredDate { get; set; }
        public string EnteredBy { get; set; }
        public string? FullName { get; set; }
        public bool Status { get; set; }
        public string StatusText
        {
            get
            {
                return Status ? "Active" : "Inactive";
            }
        }
    }

    public class StudentViewModel
    {
        public StudentDTO Student { get; set; }
        public IEnumerable<StudentDTO> Students { get; set; } = new List<StudentDTO>();
        public IEnumerable<ClassDTO> Classes { get; set; } = new List<ClassDTO>();
    }
}
