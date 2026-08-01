using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Roles
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Role name is required.")]
        public string Name { get; set; } = null!;
    }
}