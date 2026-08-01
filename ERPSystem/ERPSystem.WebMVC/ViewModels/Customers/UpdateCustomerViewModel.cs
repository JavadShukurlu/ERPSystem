using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Customers
{
    public class UpdateCustomerViewModel
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? ImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
