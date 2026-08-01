namespace ERPSystem.Application.DTOs.ModulePermissions
{
    public class UpdateModulePermissionsDto
    {
        public string UserId { get; set; } = null!;

        public string ModuleName { get; set; } = null!;

        public List<UpdateModulePermissionDto> Permissions { get; set; } = new();
    }
}