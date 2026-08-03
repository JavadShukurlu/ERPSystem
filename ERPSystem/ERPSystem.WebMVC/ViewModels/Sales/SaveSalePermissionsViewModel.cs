using ERPSystem.WebMVC.ViewModels.ModulePermissions;

namespace ERPSystem.WebMVC.ViewModels.Sales
{
    public class SaveSalePermissionsViewModel
    {
        public string UserId { get; set; } = null!;

        public List<ModulePermissionViewModel> Permissions { get; set; } = new();
    }
}