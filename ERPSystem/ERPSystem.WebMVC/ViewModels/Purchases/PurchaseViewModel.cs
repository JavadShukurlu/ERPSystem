namespace ERPSystem.WebMVC.ViewModels.Purchases
{
    public class PurchaseViewModel
    {
        public int Id { get; set; }

        public DateTime PurchaseDate { get; set; }

        public int SupplierId { get; set; }

        public string? SupplierName { get; set; }

        public decimal TotalAmount { get; set; }

        public string? CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public List<PurchaseItemViewModel> Items { get; set; } = new();
    }
}