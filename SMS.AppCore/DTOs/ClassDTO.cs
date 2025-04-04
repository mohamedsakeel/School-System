﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.DTOs
{
    public class ClassDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EnteredDate { get; set; }
        public bool Status { get; set; }

        public string StatusText
        {
            get
            {
                return Status ? "Active" : "Inactive";
            }
        }
        public List<SubjectDTO> Subjects { get; set; } = new List<SubjectDTO>();
    }

    public class ClassViewModel
    {
        public ClassDTO Class { get; set; }
        public IEnumerable<ClassDTO> Classes { get; set; } = new List<ClassDTO>();
        public IEnumerable<SubjectDTO> Subjects { get; set; } = new List<SubjectDTO>();
        public List<int> SelectedSubjectIds { get; set; } = new List<int>(); // For multiselect dropdown
    }
}
