using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SMS.AppCore.DTOs;
using SMS.AppCore.Interfaces;
using SMS.Domain.Entities;
using SMS.Infrastructure;
using Syncfusion.XlsIO;
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
        private readonly IProgressNotifier _progressNotifier;

        public StudentRepository(ApplicationDbContext dbContext, IProgressNotifier progressNotifier)
        {
            _dbContext = dbContext;
            _progressNotifier = progressNotifier;
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

        public async Task<DBResultStatus> DeleteStudent(int studentId)
        {
            if (studentId != 0)
            {
                var student = await _dbContext.Students.Where(x => x.Id == studentId).FirstOrDefaultAsync();

                if (student != null)
                {
                    student.Status = false;
                    _dbContext.Students.Update(student);
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
                    return DBResultStatus.ERROR;
                }
            }
            else
            {
                return DBResultStatus.ERROR;
            }
        }



        //Implement Bulk Upload students
        public async Task<List<Student>> ReadStudentsFromExcelAsync(IFormFile file)
        {
            var students = new List<Student>();

            using (MemoryStream stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Xlsx;

                    IWorkbook workbook = application.Workbooks.Open(stream);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    int rowCount = worksheet.UsedRange.LastRow;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        //int indexNo;
                        var indexNo = (int)worksheet[row, 1].Number; // Trim any whitespace
                        var Date = worksheet[row, 6].DateTime; // Trim any whitespace
                        var Tel = worksheet[row, 5].Text.Trim(); // Trim any whitespace
                        


                        string className = worksheet[row, 7].Text.Trim();
                        var classEntity = await _dbContext.Classes.FirstOrDefaultAsync(c => c.Name == className);
                        if (classEntity == null) continue;

                        var student = new Student
                        {
                            IndexNo = indexNo,
                            FirstName = worksheet[row, 2].Text.Trim(),
                            LastName = worksheet[row, 3].Text.Trim(),
                            Gender = worksheet[row, 4].Text.Trim(),
                            PhoneNumber = worksheet[row, 5].Text.Trim(),
                            DateOfBirth = worksheet[row, 6].DateTime,
                            ClassId = classEntity.Id,
                            EnteredDate = DateTime.Now,
                            EnteredBy = "System",
                            Status = true
                        };

                        students.Add(student);
                    }
                }
            }

            return students;
        }

        public async Task<bool> SaveStudentsAsync(List<Student> students)
        {
            int total = students.Count;
            for (int i = 0; i < total; i++)
            {
                await _dbContext.Students.AddAsync(students[i]);

                // Send Progress Update
                await _progressNotifier.SendProgress($"Uploading {i + 1}/{total}", (i + 1) * 100 / total);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
