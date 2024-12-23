using Microsoft.Data.SqlClient;
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
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public StudentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<StudentDTO>> GetAllStudentsWithClassAsync()
        {
            var studentWithClasses = await (from student in _dbContext.Students
                                            join classEntity in _dbContext.Classes on student.ClassId equals classEntity.Id
                                            select new StudentDTO
                                            {
                                                Id = student.Id,
                                                IndexNo = student.IndexNo,
                                                FirstName = student.FirstName,
                                                LastName = student.LastName,
                                                Gender = student.Gender,
                                                PhoneNumber = student.PhoneNumber,
                                                DateOfBirth = student.DateOfBirth,
                                                ClassId = student.ClassId,
                                                ClassName = classEntity.Name,
                                                EnteredDate = student.EnteredDate,
                                                EnteredBy = student.EnteredBy,
                                                Status = student.Status

                                            }).ToListAsync();

            return studentWithClasses;
        }

        public async Task<Student> GetStudentById(int Id)
        {
            return await _dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == Id);
        }

        public async Task<DBResultStatus> SaveStudent(Student Model)
        {
            var exists = await _dbContext.Students.Where(s => s.IndexNo == Model.IndexNo && s.Id != Model.Id).FirstOrDefaultAsync();

            if (exists != null)
            {
                return DBResultStatus.DUPLICATE;
            }

            Student classs = await GetStudentById(Model.Id);
            if (classs != null)
            {
                _dbContext.Students.Update(Model);
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
                await _dbContext.Students.AddAsync(Model);
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

       

        //Implement Bulk Upload students
    }
}
