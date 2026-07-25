using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Stocks
{
    public class StockDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public int WarehouseId { get; set; }

        public string? WarehouseName { get; set; }

        public int Quantity { get; set; }

        public int MinimumQuantity { get; set; }

        public bool IsLowStock => Quantity <= MinimumQuantity;
    }
}
