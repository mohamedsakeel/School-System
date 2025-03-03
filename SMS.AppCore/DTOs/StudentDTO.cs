using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.DTOs
{
    public class StudentDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Index Number is required")]
        public int IndexNo { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone Number must be 10 digits")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Class selection is required")]
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
