using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Controllers
{
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
                return View(model);
            }

            var response = await _apiService.PostAsync<LoginViewModel, ApiResponse<AuthResponseViewModel>>(
                "api/Auth/login",
                model);

            if (response is null || !response.IsSuccess || response.Data is null)
            {
                ViewBag.Error = response?.Message ?? "Login failed.";
                return View(model);
            }

            HttpContext.Session.SetString("JWToken", response.Data.Token);
            HttpContext.Session.SetString("UserName", response.Data.UserName);

            var isAdmin = response.Data.Roles.Any(role => role == "Admin");

            HttpContext.Session.SetString("IsAdmin", isAdmin ? "true" : "false");

            return RedirectToAction("Index", "Workspace");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Auth");
        }
    }
}