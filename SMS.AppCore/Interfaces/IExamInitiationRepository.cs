using SMS.Domain.Entities;

namespace SMS.AppCore.Interfaces
{
    public interface IExamInitiationRepository
    {
        Task<List<Exam>> GetExamsAsync();
        Task<ExamInitiation> GetInitiationByExamIdAsync(int examId);
        Task InitiateExamAsync(int examId, string initiatedBy);
    }
}