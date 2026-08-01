using ERPSystem.WebMVC.ViewModels.ModulePermissions;
using ERPSystem.WebMVC.ViewModels.Users;

namespace ERPSystem.WebMVC.ViewModels.Customers
{
    public class CustomerPermissionPageViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();

        public string? SelectedUserId { get; set; }

        public List<ModulePermissionViewModel> Permissions { get; set; } = new();

        public List<string> Actions { get; set; } = new()
        {
            "View",
            "Create",
            "Update",
            "Delete"
        };
    }
}