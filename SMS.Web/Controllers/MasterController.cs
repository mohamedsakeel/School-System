using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SMS.AppCore.DTOs;
using SMS.AppCore.Interfaces;
using static SMS.AppCore.Enumerations;
using System.Security.Claims;
using SMS.Domain.Entities;

namespace SMS.Web.Controllers
{
    public class MasterController : Controller
    {
        private readonly IClassRepository _classRepo;
        private readonly IMapper _mapper;
        public MasterController(IClassRepository classRepo,
                                IMapper mapper)
        {
            _classRepo = classRepo;
            _mapper = mapper;
        }

        public async Task<IActionResult> ClassMaster()
        {
            ClassViewModel VM = new ClassViewModel();
            var _class = await _classRepo.GetAllClassAsync();
            VM.Classes = _mapper.Map<IEnumerable<ClassDTO>>(_class);
            return View(VM);
        }

        [HttpPost]
        public async Task<IActionResult> SaveClass(ClassViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var _class = _mapper.Map<Class>(model.Class);

            _class.Status = model.Class.Status;
            _class.EnteredDate = DateTime.UtcNow;

            DBResultStatus res = await _classRepo.SaveClass(_class);

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
    }
}
