using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SMS.AppCore.DTOs;
using SMS.AppCore.Interfaces;
using SMS.Domain.Entities;
using SMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static SMS.AppCore.Enumerations;

namespace SMS.AppCore.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ClassRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Class>> GetAllClassAsync()
        {
            return await _dbContext.Classes.ToListAsync();
        }

        public async Task<IEnumerable<ClassDTO>> GetAllClassesWithSubjectsAsync()
        {
            var classesWithSubjects = await (from c in _dbContext.Classes
                                             join cs in _dbContext.ClassSubjects on c.Id equals cs.ClassId into classSubjects
                                             from csLeft in classSubjects.DefaultIfEmpty()
                                             join s in _dbContext.Subjects on csLeft.SubjectId equals s.Id into subjects
                                             from sLeft in subjects.DefaultIfEmpty()
                                             group sLeft by new { c.Id, c.Name, c.EnteredDate, c.Status } into classGroup
                                             select new ClassDTO
                                             {
                                                 Id = classGroup.Key.Id,
                                                 Name = classGroup.Key.Name,
                                                 EnteredDate = classGroup.Key.EnteredDate,
                                                 Status = classGroup.Key.Status,
                                                 Subjects = classGroup
                                                    .Where(sub => sub != null) // Filter out null subjects
                                                    .Select(sub => new SubjectDTO
                                                    {
                                                        Id = sub.Id,
                                                        SubjectName = sub.SubjectName,
                                                        Status = sub.Status
                                                    }).ToList()
                                             }).ToListAsync();

            return classesWithSubjects;
        }


        public async Task<Class> GetClassById(int Id)
        {
            return await _dbContext.Classes.AsNoTracking().FirstOrDefaultAsync(s => s.Id == Id);
        }

        public async Task<DBResultStatus> SaveClass(Class Model, List<int> selectedSubjects)
        {
            var exists = await _dbContext.Classes.Where(s => s.Name == Model.Name && s.Id != Model.Id).FirstOrDefaultAsync();

            if (exists != null)
            {
                return DBResultStatus.DUPLICATE;
            }

            Class classs = await GetClassById(Model.Id);
            if (classs != null)
            {
                _dbContext.Classes.Update(Model);

                var existingSubjects = await _dbContext.ClassSubjects.Where(cs => cs.ClassId == Model.Id).ToListAsync();

                _dbContext.ClassSubjects.RemoveRange(existingSubjects);

                if (selectedSubjects != null && selectedSubjects.Any())
                {
                    var classSubjects = selectedSubjects.Select(subject => new ClassSubject
                    {
                        ClassId = Model.Id,
                        SubjectId = subject
                    });

                    await _dbContext.ClassSubjects.AddRangeAsync(classSubjects);
                }

                if (await _dbContext.SaveEntityAsync() != 0)
                {
                    return DBResultStatus.SUCCESS;
                }
                else
                {
                    return DBResultStatus.DBERROR;
                }
            }
            else
            {
                await _dbContext.Classes.AddAsync(Model);
                if (await _dbContext.SaveEntityAsync() != 0)
                {
                    // After saving the class, assign the subjects
                    if (selectedSubjects != null && selectedSubjects.Any())
                    {
                        var classSubjects = selectedSubjects.Select(subjectId => new ClassSubject
                        {
                            ClassId = Model.Id,
                            SubjectId = subjectId
                        });

                        await _dbContext.ClassSubjects.AddRangeAsync(classSubjects);
                        await _dbContext.SaveEntityAsync(); // Save the ClassSubjects
                    }
                    return DBResultStatus.SUCCESS;
                }
                else
                {
                    return DBResultStatus.DBERROR;
                }
            }
        }

        //Assign Subjects to class
        public async Task AssignSubjectsToClass(int classId, List<int> subjectIds)
        {
            // Remove existing subjects assigned to the class
            var existingSubjects = _dbContext.ClassSubjects.Where(cs => cs.ClassId == classId);
            _dbContext.ClassSubjects.RemoveRange(existingSubjects);

            // Add new subject assignments
            foreach (var subjectId in subjectIds)
            {
                _dbContext.ClassSubjects.Add(new ClassSubject
                {
                    ClassId = classId,
                    SubjectId = subjectId
                });
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
