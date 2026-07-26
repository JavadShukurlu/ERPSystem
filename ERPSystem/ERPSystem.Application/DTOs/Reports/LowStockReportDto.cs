using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Reports
{
    public class LowStockReportDto
    {
        public int StockId { get; set; }

        public string ProductName { get; set; } = null!;

        public string WarehouseName { get; set; } = null!;

        public int Quantity { get; set; }

        public int MinimumQuantity { get; set; }
    }
}
