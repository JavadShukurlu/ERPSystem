using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPSystem.WebMVC.ViewModels.Users
{
    public class AssignUserRolesViewModel
    {
        public string UserId { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string? UserName { get; set; }

        public List<string> Roles { get; set; } = new();

        public List<SelectListItem> AvailableRoles { get; set; } = new();
    }
}