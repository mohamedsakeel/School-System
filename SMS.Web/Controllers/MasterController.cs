using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SMS.AppCore.DTOs;
using SMS.AppCore.Interfaces;
using static SMS.AppCore.Enumerations;
using System.Security.Claims;
using SMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using SMS.AppCore.Repositories;
using Microsoft.AspNetCore.Identity;

namespace SMS.Web.Controllers
{
    [Authorize]
    public class MasterController : Controller
    {
        private readonly IClassRepository _classRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly ISubjectRepository _subjectRepo;
        private readonly IExamRepository _examRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepo;
        private readonly IAssignTeacherClassSubjectRepository _assignTeacherRepo;
        private readonly IMapper _mapper;
        public MasterController(IClassRepository classRepo,
                                IMapper mapper,
                                IStudentRepository studentRepo,
                                ISubjectRepository subjectRepo,
                                IExamRepository examRepo,
                                UserManager<ApplicationUser> userManager,
                                IUserRepository userRepo,
                                IAssignTeacherClassSubjectRepository assignTeacherRepo)
        {
            _classRepo = classRepo;
            _mapper = mapper;
            _studentRepo = studentRepo;
            _subjectRepo = subjectRepo;
            _examRepo = examRepo;
            _userManager = userManager;
            _userRepo = userRepo;
            _assignTeacherRepo = assignTeacherRepo;
        }

        public IActionResult BulkUpload() => View();

        public async Task<IActionResult> ClassMaster()
        {
            ClassViewModel VM = new ClassViewModel();
            var _class = await _classRepo.GetAllClassesWithSubjectsAsync();
            VM.Classes = _class;

            var _allSubjects = await _subjectRepo.GetAllSubjectAsync();
            VM.Subjects = _mapper.Map<IEnumerable<SubjectDTO>>(_allSubjects);

            return View(VM);
        }

        [HttpPost]
        public async Task<IActionResult> SaveClass(ClassViewModel model)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var _class = _mapper.Map<Class>(model.Class);

                //_class.EnteredBy = userId;
                _class.EnteredDate = DateTime.UtcNow;

                DBResultStatus res = await _classRepo.SaveClass(_class, model.SelectedSubjectIds);

                if (res == DBResultStatus.SUCCESS)
                {
                    TempData["Toast"] = "Class created / updated successfully";
                }
                else if (res == DBResultStatus.ERROR || res == DBResultStatus.DBERROR)
                {
                    TempData["Toast"] = "Something went wrong!";
                }
                else if (res == DBResultStatus.DUPLICATE)
                {
                    TempData["Toast"] = "Class already exists";
                }
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
            

            return RedirectToAction("ClassMaster");
        }


        public async Task<IActionResult> StudentMaster()
        {
            var studentWithClasses = await _studentRepo.GetAllStudentsWithClassAsync();
            var allClasses = await _classRepo.GetAllClassAsync();

            StudentViewModel VM = new StudentViewModel
            {
                Students = _mapper.Map<IEnumerable<StudentDTO>>(studentWithClasses),
                Classes = _mapper.Map<IEnumerable<ClassDTO>>(allClasses),
            };

            return View(VM);
        }

        [HttpPost]
        public async Task<IActionResult> SaveStudent(StudentViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var _student = _mapper.Map<Student>(model.Student);

            _student.EnteredBy = userId;
            _student.EnteredDate = DateTime.UtcNow;

            DBResultStatus res = await _studentRepo.SaveStudent(_student);

            if (res == DBResultStatus.SUCCESS)
            {
                TempData["Toast"] = "Student created / updated successfully";
            }
            else if (res == DBResultStatus.ERROR || res == DBResultStatus.DBERROR)
            {
                TempData["Toast"] = "Something went wrong!";
            }
            else if (res == DBResultStatus.DUPLICATE)
            {
                TempData["Toast"] = "Student already exists";
            }

            return RedirectToAction("StudentMaster");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudent([FromBody] DeleteStudentRequest request)
        {
            DBResultStatus res = await _studentRepo.DeleteStudent(request.StudentId);

            if (res == DBResultStatus.SUCCESS)
            {
                return Json(new { success = true, message = "Student deleted successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Something went wrong!" });
            }
        }

        public async Task<IActionResult> SubjectMaster()
        {
            SubjectViewModel VM = new SubjectViewModel();
            var _suvject = await _subjectRepo.GetAllSubjectAsync();
            VM.Subjects = _mapper.Map<IEnumerable<SubjectDTO>>(_suvject);
            return View(VM);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSubject(SubjectViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var _subject = _mapper.Map<Subject>(model.Subject);

            _subject.EnteredBy = userId;
            _subject.EnteredDate = DateTime.UtcNow;

            DBResultStatus res = await _subjectRepo.SaveSubject(_subject);

            if (res == DBResultStatus.SUCCESS)
            {
                TempData["Toast"] = "Subject created / updated successfully";
            }
            else if (res == DBResultStatus.ERROR || res == DBResultStatus.DBERROR)
            {
                TempData["Toast"] = "Something went wrong!";
            }
            else if (res == DBResultStatus.DUPLICATE)
            {
                TempData["Toast"] = "Subject already exists";
            }

            return RedirectToAction("SubjectMaster");
        }

        public async Task<IActionResult> ExamMaster()
        {
            ExamViewModel VM = new ExamViewModel();
            var _exam = await _examRepo.GetAllExamsAsync();
            VM.Exams = _mapper.Map<IEnumerable<ExamDTO>>(_exam);
            return View(VM);
        }

        [HttpPost]
        public async Task<IActionResult> SaveExam(ExamViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var _exam = _mapper.Map<Exam>(model.Exam);

            _exam.EnteredBy = userId;
            _exam.EnteredDate = DateTime.UtcNow;

            DBResultStatus res = await _examRepo.SaveExam(_exam);

            if (res == DBResultStatus.SUCCESS)
            {
                TempData["Toast"] = "Exam created / updated successfully";
            }
            else if (res == DBResultStatus.ERROR || res == DBResultStatus.DBERROR)
            {
                TempData["Toast"] = "Something went wrong!";
            }
            else if (res == DBResultStatus.DUPLICATE)
            {
                TempData["Toast"] = "Exam already exists";
            }

            return RedirectToAction("ExamMaster");
        }

        public async Task<IActionResult> TeacherMaster()
        {
            TeacherViewModel VM = new TeacherViewModel
            {
                Teachers = _mapper.Map<IEnumerable<UserDTO>>(await _userRepo.GetUsersByRoleAsync("Teacher")),
                Classes = _mapper.Map<IEnumerable<ClassDTO>>(await _classRepo.GetAllClassAsync()),
                Subjects = _mapper.Map<IEnumerable<SubjectDTO>>(await _subjectRepo.GetAllSubjectAsync()),
                TeacherAssignments = await _assignTeacherRepo.GetTeacherAssignmentsAsync()
            };

            return View(VM);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTeacher(TeacherViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            DBResultStatus res = await _assignTeacherRepo.SaveTeacherAssignmentsAsync(model.SelectedTeacher, model.SelectedClasses, model.SelectedSubjects);

            if (res == DBResultStatus.SUCCESS)
            {
                TempData["Toast"] = "Teacher assigned / updated successfully";
            }
            else if (res == DBResultStatus.ERROR || res == DBResultStatus.DBERROR)
            {
                TempData["Toast"] = "Something went wrong!";
            }
            else if (res == DBResultStatus.DUPLICATE)
            {
                TempData["Toast"] = "Record already exists";
            }

            return RedirectToAction("TeacherMaster");
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjectsByClass(int classId)
        {
            var subjects = await _subjectRepo.GetSubjectsByClassIdAsync(classId);
            return Json(subjects);
        }

        public class DeleteStudentRequest
        {
            public int StudentId { get; set; }
        }

        //Bulk upload Student
        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            List<Student> students = await _studentRepo.ReadStudentsFromExcelAsync(file);
            return Json(students); // Show data in table before saving
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmUpload([FromBody] List<Student> students)
        {
            bool success = await _studentRepo.SaveStudentsAsync(students);
            return success ? Ok("Upload successful") : BadRequest("Upload failed");
        }
    }
}
