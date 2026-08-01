using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Warehouses;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WarehousesController : Controller
    {
        private readonly ApiService _apiService;

        public WarehousesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            var response = await _apiService.GetAsync<ApiResponse<List<WarehouseViewModel>>>("api/Warehouses");

            var warehouses = response?.Data ?? new List<WarehouseViewModel>();

            return View(warehouses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateWarehouseViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _apiService.PostAsync<CreateWarehouseViewModel, ApiResponse<WarehouseViewModel>>(
                "api/Warehouses",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Warehouse could not be created.";
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

            var response = await _apiService.GetAsync<ApiResponse<WarehouseViewModel>>($"api/Warehouses/{id}");

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new UpdateWarehouseViewModel
            {
                Id = response.Data.Id,
                Name = response.Data.Name,
                Location = response.Data.Location,
                Description = response.Data.Description
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateWarehouseViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _apiService.PutAsync<UpdateWarehouseViewModel, ApiResponse<WarehouseViewModel>>(
                $"api/Warehouses/{model.Id}",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Warehouse could not be updated.";
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

            await _apiService.DeleteAsync<ApiResponse<bool>>($"api/Warehouses/{id}");

            return RedirectToAction(nameof(Index));
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