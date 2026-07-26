using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPSystem.Application.DTOs.Reports
{
    public class DashboardReportDto
    {
        public int TotalProducts { get; set; }

        public int TotalCustomers { get; set; }

        public int TotalSuppliers { get; set; }

        public int TotalEmployees { get; set; }

        public int TotalSales { get; set; }

        public decimal TotalSalesAmount { get; set; }

        public int TotalPurchases { get; set; }

        public decimal TotalPurchaseAmount { get; set; }

        public int LowStockProductCount { get; set; }

        public decimal TotalPaidAmount { get; set; }

        public decimal TotalUnpaidInvoiceAmount { get; set; }
    }
}
