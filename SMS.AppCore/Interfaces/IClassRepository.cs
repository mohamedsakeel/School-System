using SMS.AppCore.DTOs;
using SMS.Domain.Entities;

namespace SMS.AppCore.Interfaces
{
    public interface IClassRepository
    {
        Task<IEnumerable<Class>> GetAllClassAsync();
        Task<Class> GetClassById(int Id);
        Task<Enumerations.DBResultStatus> SaveClass(Class Model, List<int> selectedSubjects);
        Task<IEnumerable<ClassDTO>> GetAllClassesWithSubjectsAsync();
        Task AssignSubjectsToClass(int classId, List<int> subjectIds);
    }
}