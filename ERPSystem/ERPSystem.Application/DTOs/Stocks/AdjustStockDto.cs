using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Stocks
{
    public class AdjustStockDto
    {
        public int StockId { get; set; }

        public int Quantity { get; set; }
    }
}
