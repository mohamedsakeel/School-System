using Microsoft.AspNetCore.Identity;
using SMS.AppCore.DTOs;

namespace SMS.AppCore.Interfaces
{
    public interface IUserRepository
    {
        Task DeleteRoleAsync(string roleId);
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        Task<IEnumerable<UserDTO>> GetAllUserAsync();
        Task<IdentityRole> GetRoleByIdAsync(string roleId);
        Task SaveRoleAsync(string roleId, string roleName);
        Task<bool> SaveUserAsync(UserDTO userDto, string roleId);
    }
}