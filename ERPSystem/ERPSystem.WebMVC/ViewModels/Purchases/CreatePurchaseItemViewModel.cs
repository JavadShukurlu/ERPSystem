using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Purchases
{
    public class CreatePurchaseItemViewModel
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int WarehouseId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        public List<SelectListItem> Products { get; set; } = new();

        public List<SelectListItem> Warehouses { get; set; } = new();
    }
}