using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = null!;

        public Employee? Employee { get; set; }

        public ICollection<ModulePermission> ModulePermissions { get; set; } = new List<ModulePermission>();
    }
}
