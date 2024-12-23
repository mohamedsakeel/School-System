using SMS.Domain.Entities;

namespace SMS.AppCore.Interfaces
{
    public interface IExamRepository
    {
        Task<IEnumerable<Exam>> GetAllExamsAsync();
        Task<Exam> GetExamByIdAsync(int Id);
        Task<Enumerations.DBResultStatus> SaveExam(Exam Model);
    }
}