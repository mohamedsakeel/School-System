using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.AppCore.DTOs;
using SMS.AppCore.Interfaces;
using System.Security.Claims;

namespace SMS.Web.Controllers
{
    [Authorize]
    public class OperationsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IEnterMarksRepository _enterMarksRepo;

        public OperationsController(IMapper mapper,
                                    IEnterMarksRepository enterMarksRepo)
        {
            _mapper = mapper;
            _enterMarksRepo = enterMarksRepo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ExamMarks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ClassSubjectMarksViewModel VM = new ClassSubjectMarksViewModel();

            var classesSubjects = await _enterMarksRepo.GetClassesSubjectsForTeacher(userId);
            VM.ClassSubjectMarkss = _mapper.Map<IEnumerable<ClassSubjectMarksDTO>>(classesSubjects);

            return View(VM);
        }

        public async Task<IActionResult> InitiateExam([FromBody] ExamInitiationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            var examInititation = _mapper.Map<ExamInitiationDTO>(dto);
            examInititation.IsActive = true;
            examInititation.InitiatedBy = User.Identity.Name;
            examInititation.InitiatedDate = DateTime.Now;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EnterMarks(int classId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if teacher is assigned to this class
            var isTeacherAssigned = await _enterMarksRepo.IsTeacherAssigned(userId);

            if (!isTeacherAssigned)
            {
                return Forbid(); // Prevent unauthorized access
            }

            var viewModel = await _enterMarksRepo.GetMarksEntryTableAsync(userId, classId);

            

            return View(viewModel);
        }
    }
}
