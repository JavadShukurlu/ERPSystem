using ERPSystem.WebMVC.ViewModels.ModulePermissions;

namespace ERPSystem.WebMVC.ViewModels.Purchases
{
    public class SavePurchasePermissionsViewModel
    {
        public string UserId { get; set; } = null!;

        public List<ModulePermissionViewModel> Permissions { get; set; } = new();
    }
}