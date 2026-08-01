using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Customers;
using ERPSystem.WebMVC.ViewModels.Employees;
using ERPSystem.WebMVC.ViewModels.Products;
using ERPSystem.WebMVC.ViewModels.Purchases;
using ERPSystem.WebMVC.ViewModels.Reports;
using ERPSystem.WebMVC.ViewModels.Sales;
using ERPSystem.WebMVC.ViewModels.Stocks;
using ERPSystem.WebMVC.ViewModels.Suppliers;
using ERPSystem.WebMVC.ViewModels.Warehouses;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportsController : Controller
    {
        private readonly ApiService _apiService;

        public ReportsController(ApiService apiService)
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
            var warehousesResponse = await _apiService.GetAsync<ApiResponse<List<WarehouseViewModel>>>("api/Warehouses");
            var stocksResponse = await _apiService.GetAsync<ApiResponse<List<StockViewModel>>>("api/Stocks");
            var salesResponse = await _apiService.GetAsync<ApiResponse<List<SaleViewModel>>>("api/Sales");
            var purchasesResponse = await _apiService.GetAsync<ApiResponse<List<PurchaseViewModel>>>("api/Purchases");

            var products = productsResponse?.Data ?? new List<ProductViewModel>();
            var customers = customersResponse?.Data ?? new List<CustomerViewModel>();
            var suppliers = suppliersResponse?.Data ?? new List<SupplierViewModel>();
            var employees = employeesResponse?.Data ?? new List<EmployeeViewModel>();
            var warehouses = warehousesResponse?.Data ?? new List<WarehouseViewModel>();
            var stocks = stocksResponse?.Data ?? new List<StockViewModel>();
            var sales = salesResponse?.Data ?? new List<SaleViewModel>();
            var purchases = purchasesResponse?.Data ?? new List<PurchaseViewModel>();

            var model = new AdminReportViewModel
            {
                TotalProducts = products.Count,
                TotalCustomers = customers.Count,
                TotalSuppliers = suppliers.Count,
                TotalEmployees = employees.Count,
                TotalWarehouses = warehouses.Count,
                TotalStockQuantity = stocks.Sum(stock => stock.Quantity),
                LowStockCount = stocks.Count(stock => stock.Quantity <= 5),
                TotalSalesAmount = sales.Sum(sale => sale.TotalAmount),
                TotalPurchaseAmount = purchases.Sum(purchase => purchase.TotalAmount)
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