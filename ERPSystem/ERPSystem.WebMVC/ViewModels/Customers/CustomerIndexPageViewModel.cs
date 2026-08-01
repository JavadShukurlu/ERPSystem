using ERPSystem.WebMVC.ViewModels.ModulePermissions;

namespace ERPSystem.WebMVC.ViewModels.Customers
{
    public class CustomerIndexPageViewModel
    {
        public List<CustomerViewModel> Customers { get; set; } = new();

        public List<ModulePermissionViewModel> Permissions { get; set; } = new();
        public CustomerPermissionPageViewModel PermissionPage { get; set; } = new();

        public bool IsAdmin { get; set; }

        public string? CurrentUserId { get; set; }

        public bool CanCreate =>
            IsAdmin || HasPermission("Create");

        public bool CanUpdateAll =>
            IsAdmin || HasPermission("Update", 2);

        public bool CanUpdateOwn =>
            IsAdmin || HasPermission("Update", 1) || HasPermission("Update", 2);

        public bool CanDeleteAll =>
            IsAdmin || HasPermission("Delete", 2);

        public bool CanDeleteOwn =>
            IsAdmin || HasPermission("Delete", 1) || HasPermission("Delete", 2);

        public bool CanViewAccessPermissions =>
            IsAdmin;

        private bool HasPermission(string actionName)
        {
            return Permissions.Any(permission =>
                permission.ActionName == actionName &&
                permission.AccessLevel > 0);
        }

        private bool HasPermission(string actionName, int accessLevel)
        {
            return Permissions.Any(permission =>
                permission.ActionName == actionName &&
                permission.AccessLevel == accessLevel);
        }
        
    }
}