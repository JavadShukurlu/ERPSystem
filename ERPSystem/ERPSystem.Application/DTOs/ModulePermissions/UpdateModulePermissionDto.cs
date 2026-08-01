using ERPSystem.Domain.Enums;

namespace ERPSystem.Application.DTOs.ModulePermissions
{
    public class UpdateModulePermissionDto
    {
        public string UserId { get; set; } = null!;

        public string ModuleName { get; set; } = null!;

        public string ActionName { get; set; } = null!;

        public PermissionAccessLevel AccessLevel { get; set; }
    }
}