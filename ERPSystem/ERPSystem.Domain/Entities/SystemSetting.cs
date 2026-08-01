using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ERPSystem.Domain.Common;

namespace ERPSystem.Domain.Entities
{
    public class SystemSetting : BaseEntity
    {
        public string CompanyName { get; set; } = "ERP System";

        public string? CompanyEmail { get; set; }

        public string? CompanyPhone { get; set; }

        public string? Address { get; set; }

        public string Currency { get; set; } = "AZN";

        public decimal TaxPercent { get; set; }

        public string InvoicePrefix { get; set; } = "INV";

        public int LowStockThreshold { get; set; } = 5;
    }
}
