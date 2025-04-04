﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SMS.Domain.Enumeration;

namespace SMS.Domain.Entities
{
    public class ExamMarks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StudentId { get; set; } // Foreign Key to Student (manual connection)
        public int SubjectId { get; set; } // Foreign Key to Subject (manual connection)
        public int ExamId { get; set; } // Foreign Key to Exam (manual connection)
        public int Score { get; set; }
    }
}
