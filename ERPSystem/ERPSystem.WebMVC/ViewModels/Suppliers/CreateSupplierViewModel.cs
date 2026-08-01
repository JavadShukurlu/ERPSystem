using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Suppliers
{
    public class CreateSupplierViewModel
    {
        [Required]
        public string CompanyName { get; set; } = null!;

        [Required]
        public string ContactName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? LogoUrl { get; set; }

        public IFormFile? LogoFile { get; set; }
    }
}
