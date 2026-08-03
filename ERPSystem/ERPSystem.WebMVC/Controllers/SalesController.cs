using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.ModulePermissions;
using ERPSystem.WebMVC.ViewModels.Sales;
using ERPSystem.WebMVC.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Controllers
{
    public class SalesController : Controller
    {
        private const string ModuleName = "Sales";
        private readonly ApiService _apiService;

        public SalesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string? selectedUserId = null, string? activeTab = null)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var salesResponse = await _apiService.GetAsync<ApiResponse<List<SaleViewModel>>>("api/Sales");

            var permissionsResponse = await _apiService.GetAsync<ApiResponse<List<ModulePermissionViewModel>>>(
                $"api/ModulePermissions/my/module/{ModuleName}");

            var model = new SaleIndexPageViewModel
            {
                Sales = salesResponse?.Data ?? new List<SaleViewModel>(),
                Permissions = permissionsResponse?.Data ?? new List<ModulePermissionViewModel>(),
                IsAdmin = IsAdmin(),
                CurrentUserId = HttpContext.Session.GetString("UserId")
            };

            if (model.IsAdmin)
            {
                var usersResponse = await _apiService.GetAsync<ApiResponse<List<UserViewModel>>>("api/Users");
                var users = usersResponse?.Data ?? new List<UserViewModel>();

                selectedUserId ??= users.FirstOrDefault()?.Id;

                model.PermissionPage = new SalePermissionPageViewModel
                {
                    Users = users,
                    SelectedUserId = selectedUserId
                };

                if (!string.IsNullOrWhiteSpace(selectedUserId))
                {
                    var userPermissionsResponse =
                        await _apiService.GetAsync<ApiResponse<List<ModulePermissionViewModel>>>(
                            $"api/ModulePermissions/user/{selectedUserId}/module/{ModuleName}");

                    model.PermissionPage.Permissions =
                        userPermissionsResponse?.Data ?? CreateDefaultPermissions();
                }
                else
                {
                    model.PermissionPage.Permissions = CreateDefaultPermissions();
                }
            }

            ViewBag.ActiveTab = activeTab ?? "salesTab";

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _apiService.DeleteAsync<ApiResponse<bool>>($"api/Sales/{id}");

            if (response is null || !response.IsSuccess)
            {
                TempData["Error"] = response?.Message ?? "Sale could not be deleted.";
            }
            else
            {
                TempData["Success"] = "Sale deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SavePermissions(SaveSalePermissionsViewModel model)
        {
            if (!IsLoggedIn() || !IsAdmin())
            {
                return RedirectToAction(nameof(Index));
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

            if (response is null || !response.IsSuccess)
            {
                TempData["Error"] = response?.Message ?? "Permissions could not be updated.";

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

        private static List<ModulePermissionViewModel> CreateDefaultPermissions()
        {
            return new List<ModulePermissionViewModel>
            {
                new() { ActionName = "View", AccessLevel = 0 },
                new() { ActionName = "Create", AccessLevel = 0 },
                new() { ActionName = "Update", AccessLevel = 0 },
                new() { ActionName = "Delete", AccessLevel = 0 }
            };
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("JWToken"));
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("IsAdmin") == "true";
        }
    }
}