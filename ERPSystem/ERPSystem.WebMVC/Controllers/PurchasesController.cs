using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Purchases;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly ApiService _apiService;

        public PurchasesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync<ApiResponse<List<PurchaseViewModel>>>("api/Purchases");

            return View(response?.Data ?? new List<PurchaseViewModel>());
        }
    }
}
