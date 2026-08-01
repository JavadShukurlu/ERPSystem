namespace ERPSystem.WebMVC.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }

        public int TotalCustomers { get; set; }

        public int TotalSuppliers { get; set; }

        public int TotalSales { get; set; }

        public int TotalPurchases { get; set; }

        public int LowStockCount { get; set; }

        public decimal TotalSalesAmount { get; set; }

        public decimal TotalPurchaseAmount { get; set; }

        public decimal TotalUnpaidInvoiceAmount { get; set; }
    }
}