using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApiService _apiService;

        public ProductsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync<ApiResponse<List<ProductViewModel>>>("api/Products");

            return View(response?.Data ?? new List<ProductViewModel>());
        }
    }
}
