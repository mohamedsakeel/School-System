using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.AppCore.DTOs
{
    public class RoleDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class RoleViewModel
    {
        public RoleDTO Role { get; set; }
        public IEnumerable<RoleDTO> Roles { get; set; }
    }
}
