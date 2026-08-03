using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Categories;
using ERPSystem.WebMVC.ViewModels.ModulePermissions;
using ERPSystem.WebMVC.ViewModels.Products;
using ERPSystem.WebMVC.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPSystem.WebMVC.Controllers
{
    public class ProductsController : Controller
    {
        private const string ModuleName = "Products";

        private readonly ApiService _apiService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(
            ApiService apiService,
            IWebHostEnvironment webHostEnvironment)
        {
            _apiService = apiService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string? selectedUserId = null, string? activeTab = null)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var isAdmin = HttpContext.Session.GetString("IsAdmin") == "true";

            var productsResponse = await _apiService.GetAsync<ApiResponse<List<ProductViewModel>>>(
                "api/Products");

            var categoriesResponse = await _apiService.GetAsync<ApiResponse<List<CategoryViewModel>>>(
                "api/Categories");

            var myPermissionsResponse = await _apiService.GetAsync<ApiResponse<List<ModulePermissionViewModel>>>(
                $"api/ModulePermissions/my/module/{ModuleName}");

            var products = productsResponse?.IsSuccess == true
                ? productsResponse.Data ?? new List<ProductViewModel>()
                : new List<ProductViewModel>();

            var categories = categoriesResponse?.IsSuccess == true
                ? categoriesResponse.Data ?? new List<CategoryViewModel>()
                : new List<CategoryViewModel>();

            FillProductCategoryNames(products, categories);

            var permissionPage = new ProductPermissionPageViewModel();

            if (isAdmin)
            {
                var usersResponse = await _apiService.GetAsync<ApiResponse<List<UserViewModel>>>(
                    "api/Users");

                var users = (usersResponse?.Data ?? new List<UserViewModel>())
                    .OrderBy(user => user.FullName)
                    .ToList();

                var currentSelectedUserId = selectedUserId ?? users.FirstOrDefault()?.Id;

                var selectedPermissions = new List<ModulePermissionViewModel>();

                if (!string.IsNullOrWhiteSpace(currentSelectedUserId))
                {
                    var selectedPermissionsResponse = await _apiService.GetAsync<ApiResponse<List<ModulePermissionViewModel>>>(
                        $"api/ModulePermissions/user/{currentSelectedUserId}/module/{ModuleName}");

                    selectedPermissions = selectedPermissionsResponse?.Data ?? new List<ModulePermissionViewModel>();
                }

                permissionPage = new ProductPermissionPageViewModel
                {
                    Users = users,
                    SelectedUserId = currentSelectedUserId,
                    Permissions = BuildDefaultPermissions(selectedPermissions)
                };
            }

            var model = new ProductIndexPageViewModel
            {
                Products = products,
                Categories = BuildCategoryFilterOptions(categories),
                Permissions = myPermissionsResponse?.Data ?? new List<ModulePermissionViewModel>(),
                PermissionPage = permissionPage,
                IsAdmin = isAdmin,
                CurrentUserId = myPermissionsResponse?.Data?.FirstOrDefault()?.UserId
            };

            ViewBag.ActiveTab = activeTab;

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
                return RedirectToAction(nameof(Index), new { activeTab = "permissions" });
            }

            model.ModuleName = ModuleName;

            foreach (var permission in model.Permissions)
            {
                permission.UserId = model.UserId;
                permission.ModuleName = ModuleName;
            }

            var response = await _apiService.PutAsync<UpdateModulePermissionsViewModel, ApiResponse<bool>>(
                "api/ModulePermissions",
                model);

            if (response is null || !response.IsSuccess)
            {
                TempData["PermissionError"] = response?.Message ?? "Permissions could not be updated.";
                return RedirectToAction(nameof(Index), new { selectedUserId = model.UserId, activeTab = "permissions" });
            }

            TempData["PermissionSuccess"] = "Permissions updated successfully.";

            return RedirectToAction(nameof(Index), new { selectedUserId = model.UserId, activeTab = "permissions" });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new CreateProductViewModel();

            await LoadCategoriesAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(model);
                return View(model);
            }

            var imageUrl = await SaveImageAsync(model.ImageFile, "products");

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                model.ImageUrl = imageUrl;
            }

            var response = await _apiService.PostAsync<CreateProductViewModel, ApiResponse<ProductViewModel>>(
                "api/Products",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Product could not be created.";
                await LoadCategoriesAsync(model);
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

            var response = await _apiService.GetAsync<ApiResponse<ProductViewModel>>(
                $"api/Products/{id}");

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new UpdateProductViewModel
            {
                Id = response.Data.Id,
                Name = response.Data.Name,
                SKU = response.Data.SKU,
                Description = response.Data.Description,
                ImageUrl = response.Data.ImageUrl,
                PurchasePrice = response.Data.PurchasePrice,
                SalePrice = response.Data.SalePrice,
                CategoryId = response.Data.CategoryId
            };

            await LoadCategoriesAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateProductViewModel model)
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
                await LoadCategoriesAsync(model);
                return View(model);
            }

            var imageUrl = await SaveImageAsync(model.ImageFile, "products");

            if (!string.IsNullOrWhiteSpace(imageUrl))
            {
                model.ImageUrl = imageUrl;
            }

            var response = await _apiService.PutAsync<UpdateProductViewModel, ApiResponse<ProductViewModel>>(
                $"api/Products/{id}",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Product could not be updated.";
                await LoadCategoriesAsync(model);
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

            var response = await _apiService.DeleteAsync<ApiResponse<bool>>(
                $"api/Products/{id}");

            if (response is null || !response.IsSuccess)
            {
                TempData["Error"] = response?.Message ?? "Product could not be deleted.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Product deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private static void FillProductCategoryNames(
            List<ProductViewModel> products,
            List<CategoryViewModel> categories)
        {
            foreach (var product in products)
            {
                if (!string.IsNullOrWhiteSpace(product.CategoryName))
                {
                    continue;
                }

                var category = categories.FirstOrDefault(category => category.Id == product.CategoryId);

                if (category is not null)
                {
                    product.CategoryName = category.Name;
                }
            }
        }

        private static List<SelectListItem> BuildCategoryFilterOptions(List<CategoryViewModel> categories)
        {
            return categories
                .Where(category => !string.IsNullOrWhiteSpace(category.Name))
                .OrderBy(category => category.Name)
                .Select(category => new SelectListItem
                {
                    Value = category.Name,
                    Text = category.Name
                })
                .ToList();
        }

        private static List<ModulePermissionViewModel> BuildDefaultPermissions(
            List<ModulePermissionViewModel> permissions)
        {
            var actions = new[] { "View", "Create", "Update", "Delete" };

            return actions
                .Select(action =>
                {
                    var existingPermission = permissions.FirstOrDefault(permission =>
                        permission.ActionName == action);

                    return new ModulePermissionViewModel
                    {
                        Id = existingPermission?.Id ?? 0,
                        UserId = existingPermission?.UserId,
                        UserFullName = existingPermission?.UserFullName,
                        ModuleName = ModuleName,
                        ActionName = action,
                        AccessLevel = existingPermission?.AccessLevel ?? 0
                    };
                })
                .ToList();
        }

        private async Task LoadCategoriesAsync(CreateProductViewModel model)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<CategoryViewModel>>>(
                "api/Categories");

            model.Categories = (response?.Data ?? new List<CategoryViewModel>())
                .OrderBy(category => category.Name)
                .Select(category => new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name
                })
                .ToList();
        }

        private async Task LoadCategoriesAsync(UpdateProductViewModel model)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<CategoryViewModel>>>(
                "api/Categories");

            model.Categories = (response?.Data ?? new List<CategoryViewModel>())
                .OrderBy(category => category.Name)
                .Select(category => new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name
                })
                .ToList();
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