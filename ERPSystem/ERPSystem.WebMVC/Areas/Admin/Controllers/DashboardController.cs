using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.AuditLogs;
using ERPSystem.WebMVC.ViewModels.Customers;
using ERPSystem.WebMVC.ViewModels.Dashboard;
using ERPSystem.WebMVC.ViewModels.Employees;
using ERPSystem.WebMVC.ViewModels.Products;
using ERPSystem.WebMVC.ViewModels.Purchases;
using ERPSystem.WebMVC.ViewModels.Sales;
using ERPSystem.WebMVC.ViewModels.Stocks;
using ERPSystem.WebMVC.ViewModels.Suppliers;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApiService _apiService;

        public DashboardController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            var productsResponse = await _apiService.GetAsync<ApiResponse<List<ProductViewModel>>>("api/Products");
            var customersResponse = await _apiService.GetAsync<ApiResponse<List<CustomerViewModel>>>("api/Customers");
            var suppliersResponse = await _apiService.GetAsync<ApiResponse<List<SupplierViewModel>>>("api/Suppliers");
            var employeesResponse = await _apiService.GetAsync<ApiResponse<List<EmployeeViewModel>>>("api/Employees");
            var stocksResponse = await _apiService.GetAsync<ApiResponse<List<StockViewModel>>>("api/Stocks");
            var salesResponse = await _apiService.GetAsync<ApiResponse<List<SaleViewModel>>>("api/Sales");
            var purchasesResponse = await _apiService.GetAsync<ApiResponse<List<PurchaseViewModel>>>("api/Purchases");
            var auditLogsResponse = await _apiService.GetAsync<ApiResponse<List<AuditLogViewModel>>>("api/AuditLogs");

            var products = productsResponse?.Data ?? new List<ProductViewModel>();
            var customers = customersResponse?.Data ?? new List<CustomerViewModel>();
            var suppliers = suppliersResponse?.Data ?? new List<SupplierViewModel>();
            var employees = employeesResponse?.Data ?? new List<EmployeeViewModel>();
            var stocks = stocksResponse?.Data ?? new List<StockViewModel>();
            var sales = salesResponse?.Data ?? new List<SaleViewModel>();
            var purchases = purchasesResponse?.Data ?? new List<PurchaseViewModel>();
            var auditLogs = auditLogsResponse?.Data ?? new List<AuditLogViewModel>();

            var model = new AdminDashboardViewModel
            {
                TotalProducts = products.Count,
                TotalCustomers = customers.Count,
                TotalSuppliers = suppliers.Count,
                TotalEmployees = employees.Count,
                LowStockCount = stocks.Count(stock => stock.Quantity <= 5),
                TotalSalesAmount = sales.Sum(sale => sale.TotalAmount),
                TotalPurchaseAmount = purchases.Sum(purchase => purchase.TotalAmount),
                RecentAuditLogs = auditLogs
                    .OrderByDescending(log => log.ActionDate)
                    .Take(5)
                    .ToList()
            };

            return View(model);
        }

        private bool IsAdminLoggedIn()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var isAdmin = HttpContext.Session.GetString("IsAdmin");

            return !string.IsNullOrWhiteSpace(token) && isAdmin == "true";
        }

        private IActionResult RedirectToAdminLogin()
        {
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }
    }
}