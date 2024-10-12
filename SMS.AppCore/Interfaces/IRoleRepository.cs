using Microsoft.AspNetCore.Identity;

namespace SMS.AppCore.Interfaces
{
    public interface IRoleRepository
    {
        Task CreateRoleAsync(IdentityRole role);
        Task DeleteRoleAsync(string roleId);
        Task<List<IdentityRole>> GetAllRolesAsync();
        Task<IdentityRole> GetRoleByIdAsync(string roleId);
        Task UpdateRoleAsync(IdentityRole role);
    }
}