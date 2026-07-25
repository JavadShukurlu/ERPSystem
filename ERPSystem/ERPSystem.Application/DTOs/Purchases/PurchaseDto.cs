using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Purchases
{
    public class PurchaseDto
    {
        public int Id { get; set; }

        public DateTime PurchaseDate { get; set; }

        public int SupplierId { get; set; }

        public string? SupplierName { get; set; }

        public decimal TotalAmount { get; set; }

        public List<PurchaseItemDto> Items { get; set; } = new();
    }
}
