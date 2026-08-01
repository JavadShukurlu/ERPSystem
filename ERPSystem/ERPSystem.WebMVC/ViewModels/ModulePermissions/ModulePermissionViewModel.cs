namespace ERPSystem.WebMVC.ViewModels.ModulePermissions
{
    public class ModulePermissionViewModel
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public string? UserFullName { get; set; }

        public string ModuleName { get; set; } = null!;

        public string ActionName { get; set; } = null!;

        public int AccessLevel { get; set; }
    }
}