using SMS.AppCore.DTOs;

namespace SMS.AppCore.Interfaces
{
    public interface IEnterMarksRepository
    {
        Task<IEnumerable<ClassSubjectMarksDTO>> GetClassesSubjectsForTeacher(string teacherId);
        Task<ClassSubjectMarksDTO> GetMarksEntryTableAsync(string userId, int classId);
        Task<bool> IsTeacherAssigned(string userId);
    }
}