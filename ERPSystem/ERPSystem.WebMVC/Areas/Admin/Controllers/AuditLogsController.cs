using ERPSystem.WebMVC.Models;
using ERPSystem.WebMVC.Services;
using ERPSystem.WebMVC.ViewModels.AuditLogs;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuditLogsController : Controller
    {
        private readonly ApiService _apiService;

        public AuditLogsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAdminLogin();
            }

            var response = await _apiService.GetAsync<ApiResponse<List<AuditLogViewModel>>>("api/AuditLogs");

            var auditLogs = response?.Data ?? new List<AuditLogViewModel>();

            auditLogs = auditLogs
                .OrderByDescending(log => log.ActionDate)
                .ToList();

            return View(auditLogs);
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