using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Users
{
    public class AssignUserRolesDto
    {
        public string UserId { get; set; } = null!;

        public List<string> Roles { get; set; } = new();
    }
}
