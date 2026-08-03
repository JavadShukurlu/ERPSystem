using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Purchases
{
    public class UpdatePurchaseViewModel
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue)]
        public int SupplierId { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        public List<CreatePurchaseItemViewModel> Items { get; set; } = new();

        public List<SelectListItem> Suppliers { get; set; } = new();
    }
}