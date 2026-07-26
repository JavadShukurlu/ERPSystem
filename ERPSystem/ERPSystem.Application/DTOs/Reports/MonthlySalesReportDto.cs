using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Reports
{
    public class MonthlySalesReportDto
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int TotalSales { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
