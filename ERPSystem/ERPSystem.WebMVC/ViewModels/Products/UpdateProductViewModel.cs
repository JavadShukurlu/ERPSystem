using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Products
{
    public class UpdateProductViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string SKU { get; set; } = null!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal PurchasePrice { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal SalePrice { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
    }
}