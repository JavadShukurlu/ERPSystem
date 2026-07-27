using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Products
{
    public class CreateProductViewModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string SKU { get; set; } = null!;

        public string? Description { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }

        public int CategoryId { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
    }
}
