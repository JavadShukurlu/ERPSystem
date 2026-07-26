using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Sales
{
    public class CreateSaleItemDto
    {
        public int ProductId { get; set; }

        public int WarehouseId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
