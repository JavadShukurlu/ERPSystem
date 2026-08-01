using ERPSystem.Domain.Enums;

namespace ERPSystem.Application.DTOs.ModulePermissions
{
    public class UserPermissionResultDto
    {
        public string ModuleName { get; set; } = null!;

        public string ActionName { get; set; } = null!;

        public PermissionAccessLevel AccessLevel { get; set; }

        public bool CanAccess { get; set; }
    }
}