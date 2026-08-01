using ERPSystem.Domain.Common;
using ERPSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Domain.Entities
{
    public class ModulePermission : BaseEntity
    {
        public string UserId { get; set; } = null!;

        public AppUser User { get; set; } = null!;

        public string ModuleName { get; set; } = null!;

        public string ActionName { get; set; } = null!;

        public PermissionAccessLevel AccessLevel { get; set; }
    }
}
