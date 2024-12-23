using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.DTOs
{
    public class SubjectDTO
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
        public bool IsElective { get; set; } // Indicates if the subject is an elective
        public DateTime EnteredDate { get; set; }
        public bool Status { get; set; }

        public string StatusText
        {
            get
            {
                return Status ? "Active" : "Inactive";
            }
        }
    }

    public class SubjectViewModel
    {
        public SubjectDTO Subject { get; set; }
        public IEnumerable<ClassDTO> Classes { get; set; } = new List<ClassDTO>();
        public IEnumerable<SubjectDTO> Subjects { get; set; } = new List<SubjectDTO>();
    }
}
