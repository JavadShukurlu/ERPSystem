namespace ERPSystem.WebMVC.ViewModels.Sales
{
    public class SaleViewModel
    {
        public int Id { get; set; }

        public DateTime SaleDate { get; set; }

        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string? CreatedByUserId { get; set; }

        public List<SaleItemViewModel> Items { get; set; } = new();
    }
}