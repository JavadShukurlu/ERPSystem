using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Products;
using ERPSystem.WebMVC.ViewModels.Stocks;
using ERPSystem.WebMVC.ViewModels.Warehouses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StocksController : Controller
    {
        private readonly ApiService _apiService;

        public StocksController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            var response = await _apiService.GetAsync<ApiResponse<List<StockViewModel>>>("api/Stocks");

            var stocks = response?.Data ?? new List<StockViewModel>();

            return View(stocks);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            var model = new CreateStockViewModel();

            await LoadDropdownsAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStockViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(model);
                return View(model);
            }

            var response = await _apiService.PostAsync<CreateStockViewModel, ApiResponse<StockViewModel>>(
                "api/Stocks",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Stock could not be created.";
                await LoadDropdownsAsync(model);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            var response = await _apiService.GetAsync<ApiResponse<StockViewModel>>($"api/Stocks/{id}");

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new UpdateStockViewModel
            {
                Id = response.Data.Id,
                ProductId = response.Data.ProductId,
                WarehouseId = response.Data.WarehouseId,
                Quantity = response.Data.Quantity
            };

            await LoadDropdownsAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateStockViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(model);
                return View(model);
            }

            var response = await _apiService.PutAsync<UpdateStockViewModel, ApiResponse<StockViewModel>>(
                $"api/Stocks/{model.Id}",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Stock could not be updated.";
                await LoadDropdownsAsync(model);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            await _apiService.DeleteAsync<ApiResponse<bool>>($"api/Stocks/{id}");

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsAsync(CreateStockViewModel model)
        {
            var productsResponse = await _apiService.GetAsync<ApiResponse<List<ProductViewModel>>>("api/Products");
            var warehousesResponse = await _apiService.GetAsync<ApiResponse<List<WarehouseViewModel>>>("api/Warehouses");

            model.Products = productsResponse?.Data?
                .Select(product => new SelectListItem
                {
                    Value = product.Id.ToString(),
                    Text = product.Name
                })
                .ToList() ?? new List<SelectListItem>();

            model.Warehouses = warehousesResponse?.Data?
                .Select(warehouse => new SelectListItem
                {
                    Value = warehouse.Id.ToString(),
                    Text = warehouse.Name
                })
                .ToList() ?? new List<SelectListItem>();
        }

        private async Task LoadDropdownsAsync(UpdateStockViewModel model)
        {
            var productsResponse = await _apiService.GetAsync<ApiResponse<List<ProductViewModel>>>("api/Products");
            var warehousesResponse = await _apiService.GetAsync<ApiResponse<List<WarehouseViewModel>>>("api/Warehouses");

            model.Products = productsResponse?.Data?
                .Select(product => new SelectListItem
                {
                    Value = product.Id.ToString(),
                    Text = product.Name
                })
                .ToList() ?? new List<SelectListItem>();

            model.Warehouses = warehousesResponse?.Data?
                .Select(warehouse => new SelectListItem
                {
                    Value = warehouse.Id.ToString(),
                    Text = warehouse.Name
                })
                .ToList() ?? new List<SelectListItem>();
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