using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleId { get; set; }
        public List<string> RoleName { get; set; }
        public string Password { get; set; }
    }

    public class UserViewModel
    {
        public UserDTO User { get; set; }
        public IEnumerable<UserDTO> Users { get; set; }
        public IEnumerable<RoleDTO> Roles { get; set; }
        public string RoleId { get; set; }
    }
}
