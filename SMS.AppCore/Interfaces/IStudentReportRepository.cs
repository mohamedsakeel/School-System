using Microsoft.AspNetCore.Http;
using SMS.AppCore.DTOs;
using SMS.Domain.Entities;

namespace SMS.AppCore.Interfaces
{
    public interface IStudentReportRepository
    {
        Task<List<StudentReportDTO>> GetClassStudentReports(int classId);
        Task<StudentReportDTO> GetStudentReport(int studentId, int classId);

    }
}