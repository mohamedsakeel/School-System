using Microsoft.EntityFrameworkCore;
using SMS.AppCore.DTOs;
using SMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.Repositories
{
    public class StudentReportRepository : IStudentReportRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public StudentReportRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StudentReportDTO> GetStudentReport(int studentId, int classId)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null) return null;

            var subjects = await _dbContext.ClassSubjects
                .Where(cs => cs.ClassId == classId)
                .Join(_dbContext.Subjects, cs => cs.SubjectId, s => s.Id, (cs, s) => s)
                .ToListAsync();

            var marks = await _dbContext.ExamMarks
                .Where(m => m.StudentId == studentId)
                .ToListAsync();

            return new StudentReportDTO
            {
                StudentId = studentId,
                FullName = $"{student.FirstName} {student.LastName}",
                Marks = subjects.ToDictionary(
                    sub => sub.SubjectName,
                    sub => marks.FirstOrDefault(m => m.SubjectId == sub.Id)?.Score
                )
            };
        }

        public async Task<List<StudentReportDTO>> GetClassStudentReports(int classId)
        {
            var students = await _dbContext.Students.Where(s => s.ClassId == classId).ToListAsync();
            var subjects = await _dbContext.ClassSubjects
                .Where(cs => cs.ClassId == classId)
                .Join(_dbContext.Subjects, cs => cs.SubjectId, s => s.Id, (cs, s) => s)
                .ToListAsync();

            var studentReports = new List<StudentReportDTO>();

            foreach (var student in students)
            {
                var marks = await _dbContext.ExamMarks.Where(m => m.StudentId == student.Id).ToListAsync();
                var report = new StudentReportDTO
                {
                    StudentId = student.Id,
                    FullName = $"{student.FirstName} {student.LastName}",
                    Marks = subjects.ToDictionary(
                        sub => sub.SubjectName,
                        sub => marks.FirstOrDefault(m => m.SubjectId == sub.Id)?.Score
                    )
                };
                studentReports.Add(report);
            }

            return studentReports.OrderByDescending(s => s.TotalMarks).ToList();
        }
    }
}
