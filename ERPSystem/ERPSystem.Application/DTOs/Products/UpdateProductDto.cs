using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Products
{
    public class UpdateProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string SKU { get; set; } = null!;

        public string? Description { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }

        public int CategoryId { get; set; }
    }
}
