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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var _class = _mapper.Map<Class>(model.Class);

            _class.EnteredBy = userId;
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
            TeacherViewModel VM = new TeacherViewModel();

            var _teachers = await _userRepo.GetUsersByRoleAsync("Teacher");
            VM.Teachers = _mapper.Map<IEnumerable<UserDTO>>(_teachers);

            var _class = await _classRepo.GetAllClassAsync();
            VM.Classes = _mapper.Map<IEnumerable<ClassDTO>>(_class);

            var _subjects = await _subjectRepo.GetAllSubjectAsync();
            VM.Subjects = _mapper.Map<IEnumerable<SubjectDTO>>(_subjects);

            var teacherAssignments = await _assignTeacherRepo.GetTeacherAssignmentsAsync();
            VM.TeacherAssignments = teacherAssignments;

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
    }
}
