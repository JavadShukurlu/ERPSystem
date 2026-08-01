using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.SystemSettings;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SystemSettingsController : Controller
    {
        private readonly ApiService _apiService;

        public SystemSettingsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            var response = await _apiService.GetAsync<ApiResponse<SystemSettingViewModel>>("api/SystemSettings");

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                ViewBag.Error = response?.Message ?? "System settings could not be loaded.";
                return View(new SystemSettingViewModel());
            }

            return View(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SystemSettingViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _apiService.PutAsync<SystemSettingViewModel, ApiResponse<SystemSettingViewModel>>(
                "api/SystemSettings",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "System settings could not be updated.";
                return View(model);
            }

            ViewBag.Success = "System settings updated successfully.";

            return View(response.Data);
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