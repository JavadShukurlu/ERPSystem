namespace ERPSystem.Application.DTOs.Purchases
{
    public class UpdatePurchaseDto
    {
        public int Id { get; set; }

        public int SupplierId { get; set; }

        public DateTime PurchaseDate { get; set; }

        public List<CreatePurchaseItemDto> Items { get; set; } = new();
    }
}