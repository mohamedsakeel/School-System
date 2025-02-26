using SMS.AppCore.DTOs;

namespace SMS.AppCore.Interfaces
{
    public interface IEnterMarksRepository
    {
        Task<IEnumerable<ClassSubjectMarksDTO>> GetClassesSubjectsForTeacher(string teacherId);
        Task<ClassSubjectMarksDTO> GetMarksEntryTableAsync(string userId, int classId, int? examId = null);
        Task<bool> IsTeacherAssigned(string userId);
        Task<bool> SaveMarks(SaveMarksDTO model);
    }
}