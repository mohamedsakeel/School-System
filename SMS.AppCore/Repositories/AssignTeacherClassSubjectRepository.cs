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
using static SMS.AppCore.Enumerations;

namespace SMS.AppCore.Repositories
{
    public class AssignTeacherClassSubjectRepository : IAssignTeacherClassSubjectRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AssignTeacherClassSubjectRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TeacherAssignmentDTO>> GetTeacherAssignmentsAsync()
        {
            // Query teacher-class-subject assignments
            var teacherClassSubjects = await _dbContext.TeacherClassSubjects.ToListAsync();

            // Fetch related data
            var teachers = await _dbContext.Users.ToListAsync(); // Assuming Users table holds teacher data
            var classes = await _dbContext.Classes.ToListAsync();
            var subjects = await _dbContext.Subjects.ToListAsync();

            // Map data manually
            var assignments = teacherClassSubjects
                .GroupBy(t => t.TeacherId)
                .Select(g =>
                {
                    var teacher = teachers.FirstOrDefault(t => t.Id == g.Key); // Match teacher by ID
                    return new TeacherAssignmentDTO
                    {
                        TeacherId = teacher.Id,
                        TeacherName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}" : "Unknown",
                        AssignedClasses = g.Select(t => classes.FirstOrDefault(c => c.Id == t.ClassId)?.Name ?? "Unknown").Distinct(),
                        AssignedSubjects = g.Select(t => subjects.FirstOrDefault(s => s.Id == t.SubjectId)?.SubjectName ?? "Unknown").Distinct()
                    };
                });

            return assignments;
        }



        public async Task<DBResultStatus> SaveTeacherAssignmentsAsync(string teacherId, List<string> selectedClasses, List<string> selectedSubjects)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Get existing assignments for the teacher
                var existingAssignments = await _dbContext.TeacherClassSubjects
                    .Where(e => e.TeacherId == teacherId)
                    .ToListAsync();

                // Build a list of new assignments based on the selected classes and subjects
                var newAssignments = new List<TeacherClassSubject>();
                foreach (var classId in selectedClasses)
                {
                    foreach (var subjectId in selectedSubjects)
                    {
                        newAssignments.Add(new TeacherClassSubject
                        {
                            TeacherId = teacherId,
                            ClassId = int.Parse(classId),
                            SubjectId = int.Parse(subjectId)
                        });
                    }
                }

                // Identify assignments to be added
                var assignmentsToAdd = newAssignments
                    .Where(newAssignment =>
                        !existingAssignments.Any(existing =>
                            existing.ClassId == newAssignment.ClassId &&
                            existing.SubjectId == newAssignment.SubjectId))
                    .ToList();

                // Identify assignments to be removed
                var assignmentsToRemove = existingAssignments
                    .Where(existing =>
                        !newAssignments.Any(newAssignment =>
                            newAssignment.ClassId == existing.ClassId &&
                            newAssignment.SubjectId == existing.SubjectId))
                    .ToList();

                // Add new assignments
                if (assignmentsToAdd.Any())
                {
                    await _dbContext.TeacherClassSubjects.AddRangeAsync(assignmentsToAdd);
                }

                // Remove outdated assignments
                if (assignmentsToRemove.Any())
                {
                    _dbContext.TeacherClassSubjects.RemoveRange(assignmentsToRemove);
                }

                // Save changes
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return DBResultStatus.SUCCESS;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return DBResultStatus.ERROR;
            }
        }

    }
}
