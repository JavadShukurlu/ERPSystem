using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Sales
{
    public class SaleDto
    {
        public int Id { get; set; }

        public DateTime SaleDate { get; set; }

        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public decimal TotalAmount { get; set; }

        public List<SaleItemDto> Items { get; set; } = new();
    }
}
