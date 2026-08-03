using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.ModulePermissions;
using ERPSystem.WebMVC.ViewModels.Suppliers;
using ERPSystem.WebMVC.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;


namespace ERPSystem.WebMVC.Controllers
{
    public class SuppliersController : Controller
    {
        private const string ModuleName = "Suppliers";

        private readonly ApiService _apiService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SuppliersController(
            ApiService apiService,
            IWebHostEnvironment webHostEnvironment)
        {
            _apiService = apiService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string? selectedUserId, string? activeTab)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var openPermissionsTab = activeTab == "permissions" || !string.IsNullOrWhiteSpace(selectedUserId);

            var supplierResponse = await _apiService.GetAsync<ApiResponse<List<SupplierViewModel>>>(
                "api/Suppliers");

            var permissionResponse = await _apiService.GetAsync<ApiResponse<List<ModulePermissionViewModel>>>(
                $"api/ModulePermissions/my/module/{ModuleName}");

            var usersResponse = await _apiService.GetAsync<ApiResponse<List<UserViewModel>>>(
                "api/Users");

            var suppliers = supplierResponse?.Data ?? new List<SupplierViewModel>();
            var permissions = permissionResponse?.Data ?? new List<ModulePermissionViewModel>();
            var users = usersResponse?.Data ?? new List<UserViewModel>();

            selectedUserId ??= users.FirstOrDefault()?.Id;

            var selectedUserPermissions = new List<ModulePermissionViewModel>();

            if (!string.IsNullOrWhiteSpace(selectedUserId))
            {
                var selectedPermissionResponse = await _apiService.GetAsync<ApiResponse<List<ModulePermissionViewModel>>>(
                    $"api/ModulePermissions/user/{selectedUserId}/module/{ModuleName}");

                selectedUserPermissions = selectedPermissionResponse?.Data ?? new List<ModulePermissionViewModel>();
            }

            ViewBag.ActiveTab = openPermissionsTab ? "permissions" : "suppliers";

            var model = new SupplierIndexPageViewModel
            {
                Suppliers = suppliers,
                Permissions = permissions,
                IsAdmin = IsAdmin(),
                CurrentUserId = HttpContext.Session.GetString("UserId"),
                PermissionPage = new SupplierPermissionPageViewModel
                {
                    Users = users,
                    SelectedUserId = selectedUserId,
                    Permissions = BuildPermissionRows(selectedUserId, selectedUserPermissions)
                }
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(new CreateSupplierViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSupplierViewModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var logoUrl = await SaveLogoAsync(model.LogoFile);

            if (!string.IsNullOrWhiteSpace(logoUrl))
            {
                model.LogoUrl = logoUrl;
            }

            var response = await _apiService.PostAsync<CreateSupplierViewModel, ApiResponse<SupplierViewModel>>(
                "api/Suppliers",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Supplier could not be created.";
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _apiService.GetAsync<ApiResponse<SupplierViewModel>>(
                $"api/Suppliers/{id}");

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new UpdateSupplierViewModel
            {
                Id = response.Data.Id,
                CompanyName = response.Data.CompanyName,
                ContactName = response.Data.ContactName,
                Email = response.Data.Email,
                PhoneNumber = response.Data.PhoneNumber,
                Address = response.Data.Address,
                LogoUrl = response.Data.LogoUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSupplierViewModel model)
        {
            if (!IsLoggedIn())
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

            var logoUrl = await SaveLogoAsync(model.LogoFile);

            if (!string.IsNullOrWhiteSpace(logoUrl))
            {
                model.LogoUrl = logoUrl;
            }

            var response = await _apiService.PutAsync<UpdateSupplierViewModel, ApiResponse<SupplierViewModel>>(
                $"api/Suppliers/{id}",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Supplier could not be updated.";
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            await _apiService.DeleteAsync<ApiResponse<bool>>($"api/Suppliers/{id}");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SavePermissions(SaveSupplierPermissionsViewModel model)
        {
            if (!IsLoggedIn() || !IsAdmin())
            {
                TempData["Error"] = "Only admin users can update permissions.";
                return RedirectToAction(nameof(Index), new { selectedUserId = model.UserId });
            }

            var request = new UpdateModulePermissionsRequest
            {
                UserId = model.UserId,
                ModuleName = ModuleName,
                Permissions = model.Permissions.Select(permission => new UpdateModulePermissionItemRequest
                {
                    UserId = model.UserId,
                    ModuleName = ModuleName,
                    ActionName = permission.ActionName,
                    AccessLevel = permission.AccessLevel
                }).ToList()
            };

            var rawResponse = await _apiService.PutRawAsync(
                "api/ModulePermissions",
                request);

            if (!rawResponse.IsSuccessStatusCode)
            {
                TempData["Error"] =
                    $"API Error {rawResponse.StatusCode}: {rawResponse.Content}";

                return RedirectToAction(nameof(Index), new { selectedUserId = model.UserId });
            }

            TempData["Success"] = "Permissions updated successfully.";

            return RedirectToAction(nameof(Index), new
            {
                selectedUserId = model.UserId,
                activeTab = "permissions"
            });
        }
        private List<ModulePermissionViewModel> BuildPermissionRows(
            string? selectedUserId,
            List<ModulePermissionViewModel> permissions)
        {
            var actions = new List<string>
            {
                "View",
                "Create",
                "Update",
                "Delete"
            };

            return actions.Select(action =>
            {
                var permission = permissions.FirstOrDefault(item => item.ActionName == action);

                return new ModulePermissionViewModel
                {
                    Id = permission?.Id ?? 0,
                    UserId = selectedUserId ?? string.Empty,
                    ModuleName = ModuleName,
                    ActionName = action,
                    AccessLevel = permission?.AccessLevel ?? 0
                };
            }).ToList();
        }

        private async Task<string?> SaveLogoAsync(IFormFile? logoFile)
        {
            if (logoFile is null || logoFile.Length == 0)
            {
                return null;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(logoFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return null;
            }

            var uploadsFolder = Path.Combine(
                _webHostEnvironment.WebRootPath,
                "uploads",
                "suppliers");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await logoFile.CopyToAsync(stream);

            return $"/uploads/suppliers/{fileName}";
        }

        private bool IsLoggedIn()
        {
            var token = HttpContext.Session.GetString("JWToken");

            return !string.IsNullOrWhiteSpace(token);
        }

        private bool IsAdmin()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin");

            return isAdmin == "true";
        }
    }

    public class SaveSupplierPermissionsViewModel
    {
        public string UserId { get; set; } = null!;

        public List<SaveSupplierPermissionItemViewModel> Permissions { get; set; } = new();
    }

    public class SaveSupplierPermissionItemViewModel
    {
        public string ActionName { get; set; } = null!;

        public int AccessLevel { get; set; }
    }

    public class UpdateModulePermissionsRequest
    {
        public string UserId { get; set; } = null!;

        public string ModuleName { get; set; } = null!;

        public List<UpdateModulePermissionItemRequest> Permissions { get; set; } = new();
    }

    public class UpdateModulePermissionItemRequest
    {
        public string UserId { get; set; } = null!;

        public string ModuleName { get; set; } = null!;

        public string ActionName { get; set; } = null!;

        public int AccessLevel { get; set; }
    }



}