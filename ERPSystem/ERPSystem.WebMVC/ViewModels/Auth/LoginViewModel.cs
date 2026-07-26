using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required]
        public string UserNameOrEmail { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
