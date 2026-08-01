namespace ERPSystem.WebMVC.ViewModels.ModulePermissions
{
    public class UpdateModulePermissionsViewModel
    {
        public string UserId { get; set; } = null!;

        public string ModuleName { get; set; } = null!;

        public List<UpdateModulePermissionViewModel> Permissions { get; set; } = new();
    }
}