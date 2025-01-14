using SMS.AppCore.DTOs;

namespace SMS.AppCore.Interfaces
{
    public interface IAssignTeacherClassSubjectRepository
    {
        Task<Enumerations.DBResultStatus> SaveTeacherAssignmentsAsync(string teacherId, List<string> selectedClasses, List<string> selectedSubjects);
        Task<IEnumerable<TeacherAssignmentDTO>> GetTeacherAssignmentsAsync();
    }
}