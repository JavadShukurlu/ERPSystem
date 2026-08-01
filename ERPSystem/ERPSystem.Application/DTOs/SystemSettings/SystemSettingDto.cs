using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.SystemSettings
{
    public class SystemSettingDto
    {
        public int Id { get; set; }

        public string CompanyName { get; set; } = null!;

        public string? CompanyEmail { get; set; }

        public string? CompanyPhone { get; set; }

        public string? Address { get; set; }

        public string Currency { get; set; } = null!;

        public decimal TaxPercent { get; set; }

        public string InvoicePrefix { get; set; } = null!;

        public int LowStockThreshold { get; set; }
    }
}
