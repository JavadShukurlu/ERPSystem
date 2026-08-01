namespace ERPSystem.WebMVC.ViewModels.Stocks
{
    public class StockViewModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public int WarehouseId { get; set; }

        public string? WarehouseName { get; set; }

        public int Quantity { get; set; }
    }
}