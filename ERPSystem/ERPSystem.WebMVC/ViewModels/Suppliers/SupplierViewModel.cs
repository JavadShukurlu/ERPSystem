namespace ERPSystem.WebMVC.ViewModels.Suppliers
{
    public class SupplierViewModel
    {
        public int Id { get; set; }

        public string CompanyName { get; set; } = null!;

        public string ContactName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? LogoUrl { get; set; }
    }
}
