using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.ModulePermissions;
using ERPSystem.WebMVC.ViewModels.Products;
using ERPSystem.WebMVC.ViewModels.Purchases;
using ERPSystem.WebMVC.ViewModels.Suppliers;
using ERPSystem.WebMVC.ViewModels.Users;
using ERPSystem.WebMVC.ViewModels.Warehouses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPSystem.WebMVC.Controllers
{
    public class PurchasesController : Controller
    {
        private const string ModuleName = "Purchases";

        private readonly ApiService _apiService;

        public PurchasesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string? selectedUserId = null, string? activeTab = null)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var purchasesResponse = await _apiService.GetAsync<ApiResponse<List<PurchaseViewModel>>>("api/Purchases");
            var permissionsResponse = await _apiService.GetAsync<ApiResponse<List<ModulePermissionViewModel>>>(
                $"api/ModulePermissions/my/module/{ModuleName}");

            var model = new PurchaseIndexPageViewModel
            {
                Purchases = purchasesResponse?.Data ?? new List<PurchaseViewModel>(),
                Permissions = permissionsResponse?.Data ?? new List<ModulePermissionViewModel>(),
                IsAdmin = IsAdmin(),
                CurrentUserId = HttpContext.Session.GetString("UserId")
            };

            if (model.IsAdmin)
            {
                await LoadPermissionPageAsync(model, selectedUserId);
            }

            ViewBag.ActiveTab = string.IsNullOrWhiteSpace(activeTab)
    ? "purchasesTab"
    : activeTab;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var model = new CreatePurchaseViewModel
            {
                PurchaseDate = DateTime.Now
            };

            await LoadPurchaseDropdownsAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePurchaseViewModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                await LoadPurchaseDropdownsAsync(model);
                return View(model);
            }

            var request = new CreatePurchaseRequest
            {
                SupplierId = model.SupplierId,
                PurchaseDate = model.PurchaseDate,
                Items = model.Items.Select(item => new CreatePurchaseItemRequest
                {
                    ProductId = item.ProductId,
                    WarehouseId = item.WarehouseId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            var response = await _apiService.PostAsync<CreatePurchaseRequest, ApiResponse<PurchaseViewModel>>(
                "api/Purchases",
                request);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Purchase could not be created.";
                await LoadPurchaseDropdownsAsync(model);
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

            var response = await _apiService.GetAsync<ApiResponse<PurchaseViewModel>>($"api/Purchases/{id}");

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new UpdatePurchaseViewModel
            {
                Id = response.Data.Id,
                SupplierId = response.Data.SupplierId,
                PurchaseDate = response.Data.PurchaseDate,
                Items = response.Data.Items.Select(item => new CreatePurchaseItemViewModel
                {
                    ProductId = item.ProductId,
                    WarehouseId = item.WarehouseId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            if (!model.Items.Any())
            {
                model.Items.Add(new CreatePurchaseItemViewModel());
            }

            await LoadPurchaseDropdownsAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdatePurchaseViewModel model)
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
                await LoadPurchaseDropdownsAsync(model);
                return View(model);
            }

            var request = new UpdatePurchaseRequest
            {
                Id = model.Id,
                SupplierId = model.SupplierId,
                PurchaseDate = model.PurchaseDate,
                Items = model.Items.Select(item => new CreatePurchaseItemRequest
                {
                    ProductId = item.ProductId,
                    WarehouseId = item.WarehouseId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            var response = await _apiService.PutAsync<UpdatePurchaseRequest, ApiResponse<PurchaseViewModel>>(
                $"api/Purchases/{id}",
                request);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "Purchase could not be updated.";
                await LoadPurchaseDropdownsAsync(model);
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

            var response = await _apiService.DeleteAsync<ApiResponse<bool>>($"api/Purchases/{id}");

            if (response is null || !response.IsSuccess)
            {
                TempData["Error"] = response?.Message ?? "Purchase could not be deleted.";
            }
            else
            {
                TempData["Success"] = "Purchase deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SavePermissions(SavePurchasePermissionsViewModel model)
        {
            if (!IsLoggedIn() || !IsAdmin())
            {
                return RedirectToAction(nameof(Index), new
                {
                    activeTab = "permissionsTab"
                });
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

            var response = await _apiService.PutAsync<UpdateModulePermissionsRequest, ApiResponse<bool>>(
                "api/ModulePermissions",
                request);

            if (response is null)
            {
                TempData["Error"] = "API response is empty. Check API authorization or request format.";

                return RedirectToAction(nameof(Index), new
                {
                    selectedUserId = model.UserId,
                    activeTab = "permissionsTab"
                });
            }

            if (!response.IsSuccess)
            {
                TempData["Error"] = response.Message ?? "Permissions could not be updated.";

                return RedirectToAction(nameof(Index), new
                {
                    selectedUserId = model.UserId,
                    activeTab = "permissionsTab"
                });
            }

            TempData["Success"] = "Permissions updated successfully.";

            return RedirectToAction(nameof(Index), new
            {
                selectedUserId = model.UserId,
                activeTab = "permissionsTab"
            });
        }

        private async Task LoadPermissionPageAsync(PurchaseIndexPageViewModel model, string? selectedUserId)
        {
            var usersResponse = await _apiService.GetAsync<ApiResponse<List<UserViewModel>>>("api/Users");
            var users = usersResponse?.Data ?? new List<UserViewModel>();

            selectedUserId ??= users.FirstOrDefault()?.Id;

            var permissions = CreateDefaultPermissions();

            if (!string.IsNullOrWhiteSpace(selectedUserId))
            {
                var permissionResponse = await _apiService.GetAsync<ApiResponse<List<ModulePermissionViewModel>>>(
                    $"api/ModulePermissions/user/{selectedUserId}/module/{ModuleName}");

                if (permissionResponse?.Data is not null && permissionResponse.Data.Any())
                {
                    permissions = MergeWithDefaultPermissions(permissionResponse.Data);
                }
            }

            model.PermissionPage = new PurchasePermissionPageViewModel
            {
                Users = users,
                SelectedUserId = selectedUserId,
                Permissions = permissions
            };
        }

        private async Task LoadPurchaseDropdownsAsync(CreatePurchaseViewModel model)
        {
            var suppliersResponse = await _apiService.GetAsync<ApiResponse<List<SupplierViewModel>>>("api/Suppliers");
            var productsResponse = await _apiService.GetAsync<ApiResponse<List<ProductViewModel>>>("api/Products");
            var warehousesResponse = await _apiService.GetAsync<ApiResponse<List<WarehouseViewModel>>>("api/Warehouses");

            model.Suppliers = (suppliersResponse?.Data ?? new List<SupplierViewModel>())
                .Select(supplier => new SelectListItem
                {
                    Value = supplier.Id.ToString(),
                    Text = supplier.CompanyName,
                    Selected = supplier.Id == model.SupplierId
                })
                .ToList();

            var products = (productsResponse?.Data ?? new List<ProductViewModel>())
                .Select(product => new SelectListItem
                {
                    Value = product.Id.ToString(),
                    Text = product.Name
                })
                .ToList();

            var warehouses = (warehousesResponse?.Data ?? new List<WarehouseViewModel>())
                .Select(warehouse => new SelectListItem
                {
                    Value = warehouse.Id.ToString(),
                    Text = warehouse.Name
                })
                .ToList();

            foreach (var item in model.Items)
            {
                item.Products = products;
                item.Warehouses = warehouses;
            }
        }

        private async Task LoadPurchaseDropdownsAsync(UpdatePurchaseViewModel model)
        {
            var suppliersResponse = await _apiService.GetAsync<ApiResponse<List<SupplierViewModel>>>("api/Suppliers");
            var productsResponse = await _apiService.GetAsync<ApiResponse<List<ProductViewModel>>>("api/Products");
            var warehousesResponse = await _apiService.GetAsync<ApiResponse<List<WarehouseViewModel>>>("api/Warehouses");

            model.Suppliers = (suppliersResponse?.Data ?? new List<SupplierViewModel>())
                .Select(supplier => new SelectListItem
                {
                    Value = supplier.Id.ToString(),
                    Text = supplier.CompanyName,
                    Selected = supplier.Id == model.SupplierId
                })
                .ToList();

            var products = (productsResponse?.Data ?? new List<ProductViewModel>())
                .Select(product => new SelectListItem
                {
                    Value = product.Id.ToString(),
                    Text = product.Name
                })
                .ToList();

            var warehouses = (warehousesResponse?.Data ?? new List<WarehouseViewModel>())
                .Select(warehouse => new SelectListItem
                {
                    Value = warehouse.Id.ToString(),
                    Text = warehouse.Name
                })
                .ToList();

            foreach (var item in model.Items)
            {
                item.Products = products.Select(product => new SelectListItem
                {
                    Value = product.Value,
                    Text = product.Text,
                    Selected = product.Value == item.ProductId.ToString()
                }).ToList();

                item.Warehouses = warehouses.Select(warehouse => new SelectListItem
                {
                    Value = warehouse.Value,
                    Text = warehouse.Text,
                    Selected = warehouse.Value == item.WarehouseId.ToString()
                }).ToList();
            }
        }

        private static List<ModulePermissionViewModel> CreateDefaultPermissions()
        {
            return new List<ModulePermissionViewModel>
            {
                new ModulePermissionViewModel
                {
                    ActionName = "View",
                    AccessLevel = 0
                },
                new ModulePermissionViewModel
                {
                    ActionName = "Create",
                    AccessLevel = 0
                },
                new ModulePermissionViewModel
                {
                    ActionName = "Update",
                    AccessLevel = 0
                },
                new ModulePermissionViewModel
                {
                    ActionName = "Delete",
                    AccessLevel = 0
                }
            };
        }

        private static List<ModulePermissionViewModel> MergeWithDefaultPermissions(List<ModulePermissionViewModel> savedPermissions)
        {
            var permissions = CreateDefaultPermissions();

            foreach (var permission in permissions)
            {
                var savedPermission = savedPermissions.FirstOrDefault(saved =>
                    saved.ActionName == permission.ActionName);

                if (savedPermission is not null)
                {
                    permission.Id = savedPermission.Id;
                    permission.UserId = savedPermission.UserId;
                    permission.UserFullName = savedPermission.UserFullName;
                    permission.ModuleName = savedPermission.ModuleName;
                    permission.AccessLevel = savedPermission.AccessLevel;
                }
            }

            return permissions;
        }

        private bool IsLoggedIn()
        {
            var token = HttpContext.Session.GetString("JWToken");

            return !string.IsNullOrWhiteSpace(token);
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("IsAdmin") == "true";
        }

        private class CreatePurchaseRequest
        {
            public int SupplierId { get; set; }

            public DateTime PurchaseDate { get; set; }

            public List<CreatePurchaseItemRequest> Items { get; set; } = new();
        }

        private class UpdatePurchaseRequest
        {
            public int Id { get; set; }

            public int SupplierId { get; set; }

            public DateTime PurchaseDate { get; set; }

            public List<CreatePurchaseItemRequest> Items { get; set; } = new();
        }

        private class CreatePurchaseItemRequest
        {
            public int ProductId { get; set; }

            public int WarehouseId { get; set; }

            public int Quantity { get; set; }

            public decimal UnitPrice { get; set; }
        }

        private class UpdateModulePermissionsRequest
        {
            public string UserId { get; set; } = null!;

            public string ModuleName { get; set; } = null!;

            public List<UpdateModulePermissionItemRequest> Permissions { get; set; } = new();
        }

        private class UpdateModulePermissionItemRequest
        {
            public string UserId { get; set; } = null!;

            public string ModuleName { get; set; } = null!;

            public string ActionName { get; set; } = null!;

            public int AccessLevel { get; set; }
        }
    }
}