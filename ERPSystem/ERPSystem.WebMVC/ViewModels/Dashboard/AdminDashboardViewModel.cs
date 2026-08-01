using ERPSystem.WebMVC.ViewModels.AuditLogs;

namespace ERPSystem.WebMVC.ViewModels.Dashboard
{
    public class AdminDashboardViewModel
    {
        public int TotalProducts { get; set; }

        public int TotalCustomers { get; set; }

        public int TotalSuppliers { get; set; }

        public int TotalEmployees { get; set; }

        public int LowStockCount { get; set; }

        public decimal TotalSalesAmount { get; set; }

        public decimal TotalPurchaseAmount { get; set; }

        public List<AuditLogViewModel> RecentAuditLogs { get; set; } = new();
    }
}