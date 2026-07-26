using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Sales;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Controllers
{
    public class SalesController : Controller
    {
        private readonly ApiService _apiService;

        public SalesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync<ApiResponse<List<SaleViewModel>>>("api/Sales");

            return View(response?.Data ?? new List<SaleViewModel>());
        }
    }
}
