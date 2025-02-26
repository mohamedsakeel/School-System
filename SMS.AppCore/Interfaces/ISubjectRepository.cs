using SMS.AppCore.DTOs;
using SMS.Domain.Entities;

namespace SMS.AppCore.Repositories
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAllSubjectAsync();
        Task<Subject> GetSubjectById(int Id);
        Task<Enumerations.DBResultStatus> SaveSubject(Subject Model);
        Task<IEnumerable<SubjectDTO>> GetSubjectsByClassIdAsync(int classId);
    }
}