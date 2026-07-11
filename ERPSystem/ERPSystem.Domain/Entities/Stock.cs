using ERPSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Domain.Entities
{
    public class Stock:BaseEntity
    {
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public int WarehouseId { get; set; }

        public Warehouse Warehouse { get; set; } = null!;

        public int Quantity { get; set; }

        public int MinimumQuantity { get; set; }
    }
}
