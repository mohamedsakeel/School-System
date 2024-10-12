using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.DTOs
{
    public class ClassDTO
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
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

    public class ClassViewModel
    {
        public ClassDTO Class { get; set; }
        public IEnumerable<ClassDTO> Classes { get; set; } = new List<ClassDTO>();
    }
}
