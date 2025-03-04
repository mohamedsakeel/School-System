using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMS.AppCore.DTOs;
using SMS.AppCore.Interfaces;

namespace SMS.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IStudentRepository _reportRepository;

        public DashboardController(IStudentRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }
        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            DahsboardViewModel VM = new DahsboardViewModel();

            var studentcount = await _reportRepository.StudentCount();
            VM.StudentCount = studentcount;
            VM.TeacherCount = 5;

            return View(VM);
        }
    }
}