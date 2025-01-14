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
    public class ExamInitiationRepository : IExamInitiationRepository
    {
        private readonly ApplicationDbContext _dbcontext;

        public ExamInitiationRepository(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<Exam>> GetExamsAsync()
        {
            return await _dbcontext.Exams.Where(x => x.Status).OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<ExamInitiation> GetInitiationByExamIdAsync(int examId)
        {
            return await _dbcontext.ExamInitiation.FirstOrDefaultAsync(e => e.ExamId == examId);
        }

        public async Task InitiateExamAsync(int examId, string initiatedBy)
        {
            var existingInitiation = await GetInitiationByExamIdAsync(examId);

            if (existingInitiation == null)
            {
                var initiation = new ExamInitiation
                {
                    ExamId = examId,
                    IsInitiated = true,
                    InitiatedBy = initiatedBy,
                    InitiatedDate = DateTime.Now
                };
                await _dbcontext.ExamInitiation.AddAsync(initiation);
            }
            else
            {
                existingInitiation.IsInitiated = true;
                existingInitiation.InitiatedBy = initiatedBy;
                existingInitiation.InitiatedDate = DateTime.Now;
            }

            await _dbcontext.SaveChangesAsync();
        }

        public async Task<ExamInitiationDto?> GetActiveExamAsync()
        {
            return await (from initiation in _dbcontext.ExamInitiation
                          join exam in _dbcontext.Exams on initiation.ExamId equals exam.Id
                          where initiation.IsInitiated
                          select new ExamInitiationDto
                          {
                              Id = initiation.Id,
                              ExamId = initiation.ExamId,
                              ExamName = exam.Name,
                              Year = exam.Year,
                              InitiatedDate = initiation.InitiatedDate,
                              InitiatedBy = initiation.InitiatedBy,
                              IsActive = initiation.IsInitiated
                          }).FirstOrDefaultAsync();
        }
    }
}
