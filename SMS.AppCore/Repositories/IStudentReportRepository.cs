using SMS.AppCore.DTOs;

namespace SMS.AppCore.Repositories
{
    public interface IStudentReportRepository
    {
        Task<List<StudentReportDTO>> GetClassStudentReports(int classId);
        Task<StudentReportDTO> GetStudentReport(int studentId, int classId);
    }
}