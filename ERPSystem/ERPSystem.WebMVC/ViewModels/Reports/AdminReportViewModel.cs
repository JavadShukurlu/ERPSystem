namespace ERPSystem.WebMVC.ViewModels.Reports
{
    public class AdminReportViewModel
    {
        public int TotalProducts { get; set; }

        public int TotalCustomers { get; set; }

        public int TotalSuppliers { get; set; }

        public int TotalEmployees { get; set; }

        public int TotalWarehouses { get; set; }

        public int TotalStockQuantity { get; set; }

        public int LowStockCount { get; set; }

        public decimal TotalSalesAmount { get; set; }

        public decimal TotalPurchaseAmount { get; set; }
    }
}