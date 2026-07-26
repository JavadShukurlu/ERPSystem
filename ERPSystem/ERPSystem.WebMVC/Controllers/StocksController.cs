using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Stocks;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Controllers
{
    public class StocksController : Controller
    {
        private readonly ApiService _apiService;

        public StocksController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync<ApiResponse<List<StockViewModel>>>("api/Stocks");

            return View(response?.Data ?? new List<StockViewModel>());
        }
    }
}
