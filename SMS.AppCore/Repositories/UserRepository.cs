using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMS.AppCore.Interfaces;
using SMS.Domain.Entities;
using AutoMapper;
using SMS.AppCore.DTOs;

namespace SMS.AppCore.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UserRepository(ApplicationDbContext dbContext,
                              UserManager<ApplicationUser> userManager,
                              IMapper mapper,
                              RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUserAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDTOs = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userDTOs.Add(new UserDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    RoleName = roles.ToList()
                });
            }

            return userDTOs;
        }

        public async Task<bool> SaveUserAsync(UserDTO userDto, string roleId)
        {
            ApplicationUser user;

            if (!string.IsNullOrEmpty(userDto.Id))
            {
                user = await _userManager.FindByIdAsync(userDto.Id);

                if (user == null)
                    return false;

                //Update User
                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.Email = userDto.Email;
                user.PhoneNumber = userDto.PhoneNumber;

                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                    return true;
            }
            else
            {
                //Create new User
                user = new ApplicationUser
                {
                    UserName = userDto.Email,
                    Email = userDto.Email,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    PhoneNumber = userDto.PhoneNumber
                };
            }

            var createResult = await _userManager.CreateAsync(user, userDto.Password);
            if (createResult.Succeeded)
                return true;

            var RoleNames = await _roleManager.Roles.Where(r => r.Id == roleId).Select(r => r.Name).ToListAsync();

            var currentRole = await _userManager.GetRolesAsync(user);

            var rolesToAdd = RoleNames.Except(currentRole).ToList();
            var rolesToRemove = currentRole.Except(RoleNames).ToList();

            // Remove roles that are no longer selected
            if (rolesToRemove.Any())
            {
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeRolesResult.Succeeded)
                    return false; // Failed to remove roles
            }

            // Add newly selected roles
            if (rolesToAdd.Any())
            {
                var addRolesResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addRolesResult.Succeeded)
                    return false; // Failed to add roles
            }

            return true;
        }

        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return _mapper.Map<IEnumerable<RoleDTO>>(roles);
        }

        public async Task<IdentityRole> GetRoleByIdAsync(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }

        public async Task SaveRoleAsync(string roleId, string roleName)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                var role = new IdentityRole(roleName);
                await _roleManager.CreateAsync(role);
            }
            else
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role != null)
                {
                    role.Name = roleName;
                    await _roleManager.UpdateAsync(role);
                }
            }
        }

        public async Task DeleteRoleAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
        }
    }
}
