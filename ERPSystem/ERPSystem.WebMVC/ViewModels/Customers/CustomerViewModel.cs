namespace ERPSystem.WebMVC.ViewModels.Customers
{
    public class CustomerViewModel
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public string? CreatedByUserId { get; set; }

        public string? UpdatedByUserId { get; set; }
    }
}