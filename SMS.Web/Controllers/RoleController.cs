using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SMS.AppCore.DTOs;
using SMS.AppCore.Interfaces;
using SMS.Web.Models;

namespace SMS.Web.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRoleRepository _roleRepo;
        private readonly IMapper _mapper;

        public RoleController(RoleManager<IdentityRole> roleManager, IRoleRepository roleRepo, IMapper mapper)
        {
            _roleManager = roleManager;
            _roleRepo = roleRepo;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var roles = _roleManager.Roles.ToList();
            var _roles = _mapper.Map<IEnumerable<RoleDTO>>(roles);
            var viewModel = new RoleViewModel
            {
                Roles = _roles,
                Role = new RoleDTO() // Initialize for create/edit scenarios
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditRole(RoleViewModel model)
        {
            if (string.IsNullOrEmpty(model.Role.Id))
            {
                // Add new role
                var role = new IdentityRole { Name = model.Role.Name };
                await _roleManager.CreateAsync(role);
            }
            else
            {
                // Edit existing role
                var role = await _roleManager.FindByIdAsync(model.Role.Id);
                if (role != null)
                {
                    role.Name = model.Role.Name;
                    await _roleManager.UpdateAsync(role);
                }
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }
    }
}
