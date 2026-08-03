using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Sales
{
    public class CreateSaleViewModel
    {
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;

        public List<CreateSaleItemViewModel> Items { get; set; } = new()
        {
            new CreateSaleItemViewModel()
        };

        public List<SelectListItem> Customers { get; set; } = new();
    }
}