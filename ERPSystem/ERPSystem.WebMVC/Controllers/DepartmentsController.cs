using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Departments;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DepartmentsController : Controller
    {
        private readonly ApiService _apiService;

        public DepartmentsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            var response = await _apiService.GetAsync<ApiResponse<List<DepartmentViewModel>>>("api/Departments");

            var departments = response?.Data ?? new List<DepartmentViewModel>();

            return View(departments);
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
        public async Task<IActionResult> Create(CreateDepartmentViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _apiService.PostAsync<CreateDepartmentViewModel, ApiResponse<DepartmentViewModel>>(
                "api/Departments",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Department could not be created.";
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

            var response = await _apiService.GetAsync<ApiResponse<DepartmentViewModel>>($"api/Departments/{id}");

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new UpdateDepartmentViewModel
            {
                Id = response.Data.Id,
                Name = response.Data.Name,
                Description = response.Data.Description
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateDepartmentViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _apiService.PutAsync<UpdateDepartmentViewModel, ApiResponse<DepartmentViewModel>>(
                $"api/Departments/{model.Id}",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Department could not be updated.";
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

            await _apiService.DeleteAsync<ApiResponse<bool>>($"api/Departments/{id}");

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