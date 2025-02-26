using Microsoft.EntityFrameworkCore;
using SMS.AppCore.DTOs;
using SMS.AppCore.Interfaces;
using SMS.Domain.Entities;
using SMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.Repositories
{
    public class EnterMarksRepository : IEnterMarksRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EnterMarksRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Get Classes and Subjects for techer
        //techer can be get from the logged in user and in identity role can be get teacher role for techer
        public async Task<IEnumerable<ClassSubjectMarksDTO>> GetClassesSubjectsForTeacher(string teacherId)
        {
            var classSubjects = await (from teach in _dbContext.TeacherClassSubjects
                                       join user in _dbContext.Users on teach.TeacherId equals user.Id
                                       join classe in _dbContext.Classes on teach.ClassId equals classe.Id
                                       join subject in _dbContext.Subjects on teach.SubjectId equals subject.Id
                                       where teach.TeacherId == teacherId
                                       && classe.Status == true
                                       && subject.Status == true
                                       select new
                                       {
                                           ClassName = classe.Name,
                                           ClassId = classe.Id,
                                           SubjectName = subject.SubjectName,
                                           TeacherName = user.FirstName + " " + user.LastName
                                       })
                               .ToListAsync();

            var groupedSubjects = classSubjects
                .GroupBy(c => new { c.ClassId, c.ClassName, c.TeacherName })
                .Select(g => new ClassSubjectMarksDTO
                {
                    ClassId = g.Key.ClassId,
                    ClassName = g.Key.ClassName,
                    TeacherName = g.Key.TeacherName,
                    Subjects = g.Select(s => s.SubjectName).ToList()
                })
                .ToList();

            return groupedSubjects;
        }

        public async Task<ClassSubjectMarksDTO> GetMarksEntryTableAsync(string userId, int classId, int? examId = null)
        {
            var teacherAssingedSubject = await _dbContext.TeacherClassSubjects.Where(x => x.TeacherId == userId && classId == classId).Select(x => x.SubjectId).ToListAsync();

            var students = await _dbContext.Students.Where(x => x.ClassId == classId).OrderBy(s => s.IndexNo).ToListAsync();

            var subjects = await _dbContext.Subjects.Where(sub => teacherAssingedSubject.Contains(sub.Id))
                .Select(sub => new SubjectDTO
                {
                    Id = sub.Id,
                    SubjectName = sub.SubjectName
                }).ToListAsync();

            // Get saved marks for the students and subjects for the given exam
            var existingMarks = await _dbContext.ExamMarks
                .Where(em => em.ExamId == 1 && students.Select(s => s.Id).Contains(em.StudentId) && teacherAssingedSubject.Contains(em.SubjectId))
                .ToListAsync();

            var className = _dbContext.Classes.FirstOrDefault(c => c.Id == classId)?.Name;
            var teacherName = _dbContext.Users.Where(c => c.Id == userId).Select(c => c.FirstName + " " + c.LastName).FirstOrDefault();

            return new ClassSubjectMarksDTO
            {
                ClassId = classId,
                ClassName = className ?? "Unknown Class",
                TeacherName = teacherName ?? "Unknown Teacher",
                SubjectsList = subjects,
                Students = students.Select(s => new StudentMarksDTO
                {
                    Id = s.Id,
                    IndexNo = s.IndexNo,
                    FullName = s.FirstName + " " + s.LastName,
                    Marks = subjects.ToDictionary(
                sub => sub.Id,
                sub => existingMarks.FirstOrDefault(m => m.StudentId == s.Id && m.SubjectId == sub.Id)?.Score ?? 0
            ) // Assign marks if available, otherwise default to 0
                }).ToList()

            };

        }

        public async Task<bool> IsTeacherAssigned(string userId)
        {
            return await _dbContext.TeacherClassSubjects.AnyAsync(t => t.TeacherId == userId);
        }

        public async Task<bool> SaveMarks(SaveMarksDTO model)
        {
            foreach (var studentMarks in model.Marks)
            {
                int studentId = studentMarks.Key;

                foreach (var subjectMark in studentMarks.Value)
                {
                    int subjectId = subjectMark.Key;
                    int score = subjectMark.Value;

                    // Check if the mark already exists
                    var existingMark = await _dbContext.ExamMarks
                        .FirstOrDefaultAsync(m => m.StudentId == studentId && m.SubjectId == subjectId && m.ExamId == 1);

                    if (existingMark != null)
                    {
                        existingMark.Score = score; // Update existing mark
                    }
                    else
                    {
                        _dbContext.ExamMarks.Add(new ExamMarks
                        {
                            StudentId = studentId,
                            SubjectId = subjectId,
                            ExamId = 1,
                            Score = score
                        });
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
