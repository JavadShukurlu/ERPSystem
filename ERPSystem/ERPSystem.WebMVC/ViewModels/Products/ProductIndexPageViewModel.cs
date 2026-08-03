using ERPSystem.WebMVC.ViewModels.ModulePermissions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPSystem.WebMVC.ViewModels.Products
{
    public class ProductIndexPageViewModel
    {
        public List<ProductViewModel> Products { get; set; } = new();

        public List<ModulePermissionViewModel> Permissions { get; set; } = new();

        public ProductPermissionPageViewModel PermissionPage { get; set; } = new();

        public List<SelectListItem> Categories { get; set; } = new();

        public bool IsAdmin { get; set; }

        public string? CurrentUserId { get; set; }

        public bool CanView =>
            IsAdmin || HasPermission("View");

        public bool CanCreate =>
            CanView && (IsAdmin || HasPermission("Create"));

        public bool CanUpdateAll =>
            CanView && (IsAdmin || HasPermission("Update", 2));

        public bool CanUpdateOwn =>
            CanView && (IsAdmin || HasPermission("Update", 1) || HasPermission("Update", 2));

        public bool CanDeleteAll =>
            CanView && (IsAdmin || HasPermission("Delete", 2));

        public bool CanDeleteOwn =>
            CanView && (IsAdmin || HasPermission("Delete", 1) || HasPermission("Delete", 2));

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