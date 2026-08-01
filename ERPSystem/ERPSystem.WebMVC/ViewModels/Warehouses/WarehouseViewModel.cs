namespace ERPSystem.WebMVC.ViewModels.Warehouses
{
    public class WarehouseViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Location { get; set; }

        public string? Description { get; set; }
    }
}