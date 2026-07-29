using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;

        public AuthController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please fill username and password.";
                return View(model);
            }

            var response = await _apiService.PostAsync<LoginViewModel, ApiResponse<AuthResponseViewModel>>(
                "api/Auth/login",
                model);

            if (response is null)
            {
                ViewBag.Error = "Could not connect to API. Check WebAPI port and ApiSettings BaseUrl.";
                return View(model);
            }

            if (!response.IsSuccess || response.Data is null)
            {
                ViewBag.Error = response.Message ?? "Login failed.";
                return View(model);
            }

            if (!response.Data.Roles.Any(role => role.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
            {
                ViewBag.Error = "You are not allowed to access admin panel.";
                return View(model);
            }

            HttpContext.Session.SetString("JWToken", response.Data.Token);
            HttpContext.Session.SetString("UserName", response.Data.UserName);
            HttpContext.Session.SetString("IsAdmin", "true");

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }
    }
}