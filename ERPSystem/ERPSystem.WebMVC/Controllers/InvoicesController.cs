using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Invoices;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly ApiService _apiService;

        public InvoicesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync<ApiResponse<List<InvoiceViewModel>>>("api/Invoices");

            return View(response?.Data ?? new List<InvoiceViewModel>());
        }
    }
}
