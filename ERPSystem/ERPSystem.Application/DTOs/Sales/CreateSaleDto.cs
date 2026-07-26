using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Sales
{
    public class CreateSaleDto
    {
        public int CustomerId { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.UtcNow;

        public List<CreateSaleItemDto> Items { get; set; } = new();
    }
}
