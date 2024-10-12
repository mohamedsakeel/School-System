using SMS.Domain.Entities;

namespace SMS.AppCore.Interfaces
{
    public interface IClassRepository
    {
        Task<IEnumerable<Class>> GetAllClassAsync();
        Task<Class> GetClassById(int Id);
        Task<Enumerations.DBResultStatus> SaveClass(Class Model);
    }
}