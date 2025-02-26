using Microsoft.EntityFrameworkCore;
using SMS.AppCore.DTOs;
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
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SubjectRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Subject>> GetAllSubjectAsync()
        {
            return await _dbContext.Subjects.ToListAsync();
        }

        public async Task<Subject> GetSubjectById(int Id)
        {
            return await _dbContext.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == Id);
        }

        public async Task<IEnumerable<SubjectDTO>> GetSubjectsByClassIdAsync(int classId)
        {
            return await (from c in _dbContext.ClassSubjects
                         join s in _dbContext.Subjects on c.SubjectId equals s.Id
                         where c.ClassId == classId
                         select new SubjectDTO
                        {
                            Id = s.Id,
                            SubjectName = s.SubjectName
                        })
                        .ToListAsync();
        }


        public async Task<DBResultStatus> SaveSubject(Subject Model)
        {
            var exists = await _dbContext.Subjects.Where(s => s.SubjectName == Model.SubjectName && s.Id != Model.Id).FirstOrDefaultAsync();

            if (exists != null)
            {
                return DBResultStatus.DUPLICATE;
            }

            Subject subjects = await GetSubjectById(Model.Id);
            if (subjects != null)
            {
                _dbContext.Subjects.Update(Model);
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
                Model.Status = true;
                await _dbContext.Subjects.AddAsync(Model);
                if (await _dbContext.SaveEntityAsync() != 0)
                {
                    return DBResultStatus.SUCCESS;
                }
                else
                {
                    return DBResultStatus.DBERROR;
                }
            }
        }
    }
}
