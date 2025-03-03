using Microsoft.AspNetCore.Http;
using SMS.AppCore.DTOs;
using SMS.Domain.Entities;
using static SMS.AppCore.Enumerations;

namespace SMS.AppCore.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<StudentDTO>> GetAllStudentsWithClassAsync();
        Task<Student> GetStudentById(int Id);
        Task<Enumerations.DBResultStatus> SaveStudent(Student Model);
        Task<DBResultStatus> DeleteStudent(int studentId);
        Task<List<Student>> ReadStudentsFromExcelAsync(IFormFile file);
        Task<bool> SaveStudentsAsync(List<Student> students);
    }
}