using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERPSystem.WebMVC.ViewModels.Sales
{
    public class UpdateSaleViewModel
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        public DateTime SaleDate { get; set; }

        public List<UpdateSaleItemViewModel> Items { get; set; } = new();

        public List<SelectListItem> Customers { get; set; } = new();
    }
}