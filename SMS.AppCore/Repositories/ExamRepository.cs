using Microsoft.EntityFrameworkCore;
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
    public class ExamRepository : IExamRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ExamRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Exam>> GetAllExamsAsync()
        {
            return await _dbContext.Exams.ToListAsync();
        }

        public async Task<Exam> GetExamByIdAsync(int Id)
        {
            return await _dbContext.Exams.AsNoTracking().FirstOrDefaultAsync(e => e.Id == Id);
        }

        public async Task<DBResultStatus> SaveExam(Exam Model)
        {
            var exists = await _dbContext.Exams.Where(s => s.Name == Model.Name && s.Id != Model.Id && s.Year == Model.Year).FirstOrDefaultAsync();

            if (exists != null)
            {
                return DBResultStatus.DUPLICATE;
            }

            Exam exams = await GetExamByIdAsync(Model.Id);
            if (exams != null)
            {
                _dbContext.Exams.Update(Model);
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
                await _dbContext.Exams.AddAsync(Model);
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
