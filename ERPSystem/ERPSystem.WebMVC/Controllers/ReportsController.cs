using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApiService _apiService;

        public ReportsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync<ApiResponse<DashboardViewModel>>("api/Reports/dashboard");

            return View(response?.Data ?? new DashboardViewModel());
        }
    }
}
