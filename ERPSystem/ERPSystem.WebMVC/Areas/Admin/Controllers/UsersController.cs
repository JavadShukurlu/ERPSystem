using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Roles;
using ERPSystem.WebMVC.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly ApiService _apiService;

        public UsersController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            var response = await _apiService.GetAsync<ApiResponse<List<UserViewModel>>>("api/Users");

            var users = response?.Data ?? new List<UserViewModel>();

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            var model = new CreateUserViewModel();

            await LoadRolesAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            if (!ModelState.IsValid)
            {
                await LoadRolesAsync(model);
                return View(model);
            }

            var response = await _apiService.PostAsync<CreateUserViewModel, ApiResponse<UserViewModel>>(
                "api/Users",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "User could not be created.";
                await LoadRolesAsync(model);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AssignRoles(string id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            var response = await _apiService.GetAsync<ApiResponse<UserViewModel>>($"api/Users/{id}");

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new AssignUserRolesViewModel
            {
                UserId = response.Data.Id,
                FullName = response.Data.FullName,
                UserName = response.Data.UserName,
                Roles = response.Data.Roles
            };

            await LoadRolesAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoles(AssignUserRolesViewModel model)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            var response = await _apiService.PutAsync<AssignUserRolesViewModel, ApiResponse<UserViewModel>>(
                "api/Users/assign-roles",
                model);

            if (response is null || !response.IsSuccess)
            {
                ViewBag.Error = response?.Message ?? "User roles could not be updated.";
                await LoadRolesAsync(model);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            await _apiService.DeleteAsync<ApiResponse<bool>>($"api/Users/{id}");

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadRolesAsync(CreateUserViewModel model)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<RoleViewModel>>>("api/Roles");

            model.AvailableRoles = response?.Data?
                .Select(role => new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Name,
                    Selected = model.Roles.Contains(role.Name)
                })
                .ToList() ?? new List<SelectListItem>();
        }

        private async Task LoadRolesAsync(AssignUserRolesViewModel model)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<RoleViewModel>>>("api/Roles");

            model.AvailableRoles = response?.Data?
                .Select(role => new SelectListItem
                {
                    Value = role.Name,
                    Text = role.Name,
                    Selected = model.Roles.Contains(role.Name)
                })
                .ToList() ?? new List<SelectListItem>();
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