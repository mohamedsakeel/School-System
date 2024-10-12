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

        public async Task<Class> GetClassById(int Id)
        {
            return await _dbContext.Classes.AsNoTracking().FirstOrDefaultAsync(s => s.ClassId == Id);
        }

        public async Task<DBResultStatus> SaveClass(Class Model)
        {
            var exists = await _dbContext.Classes.Where(s => s.ClassName == Model.ClassName && s.ClassId != Model.ClassId).FirstOrDefaultAsync();

            if (exists != null)
            {
                return DBResultStatus.DUPLICATE;
            }

            Class classs = await GetClassById(Model.ClassId);
            if (classs != null)
            {
                _dbContext.Classes.Update(Model);
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
