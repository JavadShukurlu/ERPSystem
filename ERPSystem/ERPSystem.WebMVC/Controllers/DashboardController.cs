using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApiService _apiService;

        public DashboardController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _apiService.GetAsync<ApiResponse<DashboardViewModel>>(
                "api/Reports/dashboard");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            return View(response?.Data ?? new DashboardViewModel());
        }
    }
}
