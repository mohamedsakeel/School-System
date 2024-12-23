using SMS.Domain.Entities;

namespace SMS.AppCore.Repositories
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAllSubjectAsync();
        Task<Subject> GetSubjectById(int Id);
        Task<Enumerations.DBResultStatus> SaveSubject(Subject Model);
    }
}