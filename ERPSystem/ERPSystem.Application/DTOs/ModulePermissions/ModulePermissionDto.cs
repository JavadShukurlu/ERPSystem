using ERPSystem.Domain.Enums;

namespace ERPSystem.Application.DTOs.ModulePermissions
{
    public class ModulePermissionDto
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public string? UserFullName { get; set; }

        public string ModuleName { get; set; } = null!;

        public string ActionName { get; set; } = null!;

        public PermissionAccessLevel AccessLevel { get; set; }
    }
}