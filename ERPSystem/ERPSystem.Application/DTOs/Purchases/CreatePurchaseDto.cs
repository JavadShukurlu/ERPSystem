using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Purchases
{
    public class CreatePurchaseDto
    {
        public int SupplierId { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        public List<CreatePurchaseItemDto> Items { get; set; } = new();
    }
}
