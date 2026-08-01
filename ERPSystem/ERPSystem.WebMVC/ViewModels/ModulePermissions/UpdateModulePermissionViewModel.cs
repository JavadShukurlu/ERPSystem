namespace ERPSystem.WebMVC.ViewModels.ModulePermissions
{
    public class UpdateModulePermissionViewModel
    {
        public string UserId { get; set; } = null!;

        public string ModuleName { get; set; } = null!;

        public string ActionName { get; set; } = null!;

        public int AccessLevel { get; set; }
    }
}