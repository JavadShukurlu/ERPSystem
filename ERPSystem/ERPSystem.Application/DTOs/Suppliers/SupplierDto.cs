namespace ERPSystem.Application.DTOs.Suppliers
{
    public class SupplierDto
    {
        public int Id { get; set; }

        public string CompanyName { get; set; } = null!;

        public string ContactName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? LogoUrl { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? CreatedByUserId { get; set; }

        public string? UpdatedByUserId { get; set; }
    }
}