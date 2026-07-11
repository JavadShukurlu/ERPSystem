using ERPSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Domain.Entities
{
    public class Warehouse:BaseEntity
    {
        public string Name { get; set; } = null!;

        public string Location { get; set; } = null!;

        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
}
