namespace ERPSystem.Application.DTOs.Purchases
{
    public class PurchaseDto
    {
        public int Id { get; set; }

        public DateTime PurchaseDate { get; set; }

        public int SupplierId { get; set; }

        public string? SupplierName { get; set; }

        public decimal TotalAmount { get; set; }

        public string? CreatedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public List<PurchaseItemDto> Items { get; set; } = new();
    }
}