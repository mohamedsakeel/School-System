using SMS.AppCore.DTOs;
using SMS.Domain.Entities;

namespace SMS.AppCore.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<StudentDTO>> GetAllStudentsWithClassAsync();
        Task<Student> GetStudentById(int Id);
        Task<Enumerations.DBResultStatus> SaveStudent(Student Model);
    }
}