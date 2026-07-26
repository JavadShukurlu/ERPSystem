namespace ERPSystem.WebMVC.ViewModels.Dashboard
{
    public class DashboardViewModel
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
