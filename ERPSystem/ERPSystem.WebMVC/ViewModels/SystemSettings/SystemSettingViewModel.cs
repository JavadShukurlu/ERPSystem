using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.SystemSettings
{
    public class SystemSettingViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        public string CompanyName { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Email format is not valid.")]
        public string? CompanyEmail { get; set; }

        public string? CompanyPhone { get; set; }

        public string? Address { get; set; }

        [Required(ErrorMessage = "Currency is required.")]
        public string Currency { get; set; } = null!;

        [Range(0, 100, ErrorMessage = "Tax percent must be between 0 and 100.")]
        public decimal TaxPercent { get; set; }

        [Required(ErrorMessage = "Invoice prefix is required.")]
        public string InvoicePrefix { get; set; } = null!;

        [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold cannot be negative.")]
        public int LowStockThreshold { get; set; }
    }
}