using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Customers;
using ERPSystem.WebMVC.ViewModels.ModulePermissions;
using ERPSystem.WebMVC.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Controllers
{
    public class CustomersController : Controller
    {
        private const string ModuleName = "Customers";

        private readonly ApiService _apiService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CustomersController(
            ApiService apiService,
            IWebHostEnvironment webHostEnvironment)
        {
            _apiService = apiService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string? selectedUserId = null)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";

            var customersResponse = await _apiService.GetAsync<ApiResponse<List<CustomerViewModel>>>(
                "api/Customers");

            var myPermissionsResponse = await _apiService.GetAsync<ApiResponse<List<ModulePermissionViewModel>>>(
                "api/ModulePermissions/my/module/Customers");

            var permissionPage = new CustomerPermissionPageViewModel();

            if (isAdmin)
            {
                var usersResponse = await _apiService.GetAsync<ApiResponse<List<UserViewModel>>>(
                    "api/Users");

                var users = usersResponse?.Data ?? new List<UserViewModel>();
                var currentSelectedUserId = selectedUserId ?? users.FirstOrDefault()?.Id;

                var selectedPermissions = new List<ModulePermissionViewModel>();

                if (!string.IsNullOrWhiteSpace(currentSelectedUserId))
                {
                    var selectedPermissionsResponse = await _apiService.GetAsync<ApiResponse<List<ModulePermissionViewModel>>>(
                        $"api/ModulePermissions/user/{currentSelectedUserId}/module/{ModuleName}");

                    selectedPermissions = selectedPermissionsResponse?.Data ?? new List<ModulePermissionViewModel>();
                }

                permissionPage = new CustomerPermissionPageViewModel
                {
                    Users = users,
                    SelectedUserId = currentSelectedUserId,
                    Permissions = selectedPermissions
                };
            }

            var model = new CustomerIndexPageViewModel
            {
                Customers = customersResponse?.Data ?? new List<CustomerViewModel>(),
                Permissions = myPermissionsResponse?.Data ?? new List<ModulePermissionViewModel>(),
                PermissionPage = permissionPage,
                IsAdmin = isAdmin,
                CurrentUserId = myPermissionsResponse?.Data?.FirstOrDefault()?.UserId
            };

            return View(model);
        }

        
        [HttpPost]
        public async Task<IActionResult> SavePermissions(UpdateModulePermissionsViewModel model)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!isAdmin)
            {
                TempData["PermissionError"] = "Only admin users can update permissions.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(model.UserId))
            {
                TempData["PermissionError"] = "Selected user was not found.";
                return RedirectToAction(nameof(Index));
            }

            model.ModuleName = "Customers";

            var actions = new[] { "View", "Create", "Update", "Delete" };

            foreach (var action in actions)
            {
                var permission = model.Permissions.FirstOrDefault(x => x.ActionName == action);

                if (permission is not null)
                {
                    permission.UserId = model.UserId;
                    permission.ModuleName = "Customers";
                }
            }

            var response = await _apiService.PutAsync<UpdateModulePermissionsViewModel, ApiResponse<bool>>(
                "api/ModulePermissions",
                model);

            if (response is null)
            {
                TempData["PermissionError"] = "API response is null. Permissions were not saved.";
                return RedirectToAction(nameof(Index), new { selectedUserId = model.UserId });
            }

            if (!response.IsSuccess)
            {
                TempData["PermissionError"] = response.Message ?? "Permissions could not be updated.";
                return RedirectToAction(nameof(Index), new { selectedUserId = model.UserId });
            }

            TempData["PermissionSuccess"] = "Permissions updated successfully.";

            return RedirectToAction(nameof(Index), new { selectedUserId = model.UserId });
        }

        [HttpGet]
        public IActionResult Create()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerViewModel model)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var imageUrl = await SaveImageAsync(model.ImageFile, "customers");

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                model.ImageUrl = imageUrl;
            }

            var response = await _apiService.PostAsync<CreateCustomerViewModel, ApiResponse<CustomerViewModel>>(
                "api/Customers",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Customer could not be created.";
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _apiService.GetAsync<ApiResponse<CustomerViewModel>>(
                $"api/Customers/{id}");

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new UpdateCustomerViewModel
            {
                Id = response.Data.Id,
                FullName = response.Data.FullName,
                Email = response.Data.Email,
                PhoneNumber = response.Data.PhoneNumber,
                Address = response.Data.Address,
                ImageUrl = response.Data.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateCustomerViewModel model)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (id != model.Id)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var imageUrl = await SaveImageAsync(model.ImageFile, "customers");

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                model.ImageUrl = imageUrl;
            }

            var response = await _apiService.PutAsync<UpdateCustomerViewModel, ApiResponse<CustomerViewModel>>(
                $"api/Customers/{id}",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Customer could not be updated.";
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            await _apiService.DeleteAsync<ApiResponse<bool>>($"api/Customers/{id}");

            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> SaveImageAsync(IFormFile? imageFile, string folderName)
        {
            if (imageFile is null || imageFile.Length == 0)
            {
                return null;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return null;
            }

            var uploadsFolder = Path.Combine(
                _webHostEnvironment.WebRootPath,
                "uploads",
                folderName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/uploads/{folderName}/{fileName}";
        }
    }
}