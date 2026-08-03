namespace ERPSystem.Application.DTOs.Suppliers
{
    public class CreateSupplierDto
    {
        public string CompanyName { get; set; } = null!;

        public string ContactName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? LogoUrl { get; set; }
    }
}