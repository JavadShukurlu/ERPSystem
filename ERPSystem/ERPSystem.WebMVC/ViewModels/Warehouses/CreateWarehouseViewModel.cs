using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Warehouses
{
    public class CreateWarehouseViewModel
    {
        [Required(ErrorMessage = "Warehouse name is required.")]
        public string Name { get; set; } = null!;

        public string? Location { get; set; }

        public string? Description { get; set; }
    }
}