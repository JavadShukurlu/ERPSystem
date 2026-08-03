using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Purchases
{
    public class CreatePurchaseViewModel
    {
        [Range(1, int.MaxValue)]
        public int SupplierId { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        public List<CreatePurchaseItemViewModel> Items { get; set; } = new()
        {
            new CreatePurchaseItemViewModel()
        };

        public List<SelectListItem> Suppliers { get; set; } = new();
    }
}