using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Departments;
using ERPSystem.WebMVC.ViewModels.Employees;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeesController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeesController(ApiService apiService, IWebHostEnvironment webHostEnvironment)
        {
            _apiService = apiService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            var response = await _apiService.GetAsync<ApiResponse<List<EmployeeViewModel>>>("api/Employees");

            var employees = response?.Data ?? new List<EmployeeViewModel>();

            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            var model = new CreateEmployeeViewModel
            {
                HireDate = DateTime.Now
            };

            await LoadDepartmentsAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync(model);
                return View(model);
            }

            var imageUrl = await SaveImageAsync(model.ImageFile, "employees");

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                model.ImageUrl = imageUrl;
            }

            var response = await _apiService.PostAsync<CreateEmployeeViewModel, ApiResponse<EmployeeViewModel>>(
                "api/Employees",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Employee could not be created.";
                await LoadDepartmentsAsync(model);
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

            var response = await _apiService.GetAsync<ApiResponse<EmployeeViewModel>>($"api/Employees/{id}");

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new UpdateEmployeeViewModel
            {
                Id = response.Data.Id,
                FirstName = response.Data.FirstName,
                LastName = response.Data.LastName,
                Email = response.Data.Email,
                PhoneNumber = response.Data.PhoneNumber,
                Position = response.Data.Position,
                Salary = response.Data.Salary,
                HireDate = response.Data.HireDate,
                ImageUrl = response.Data.ImageUrl,
                DepartmentId = response.Data.DepartmentId
            };

            await LoadDepartmentsAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateEmployeeViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync(model);
                return View(model);
            }

            var imageUrl = await SaveImageAsync(model.ImageFile, "employees");

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                model.ImageUrl = imageUrl;
            }

            var response = await _apiService.PutAsync<UpdateEmployeeViewModel, ApiResponse<EmployeeViewModel>>(
                $"api/Employees/{model.Id}",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Employee could not be updated.";
                await LoadDepartmentsAsync(model);
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

            await _apiService.DeleteAsync<ApiResponse<bool>>($"api/Employees/{id}");

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDepartmentsAsync(CreateEmployeeViewModel model)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<DepartmentViewModel>>>("api/Departments");

            model.Departments = response?.Data?
                .Select(department => new SelectListItem
                {
                    Value = department.Id.ToString(),
                    Text = department.Name
                })
                .ToList() ?? new List<SelectListItem>();
        }

        private async Task LoadDepartmentsAsync(UpdateEmployeeViewModel model)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<DepartmentViewModel>>>("api/Departments");

            model.Departments = response?.Data?
                .Select(department => new SelectListItem
                {
                    Value = department.Id.ToString(),
                    Text = department.Name
                })
                .ToList() ?? new List<SelectListItem>();
        }

        private async Task<string?> SaveImageAsync(IFormFile? file, string folderName)
        {
            if (file is null || file.Length == 0)
            {
                return null;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return null;
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folderName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{folderName}/{fileName}";
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