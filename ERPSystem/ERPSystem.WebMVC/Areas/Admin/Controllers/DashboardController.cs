using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var isAdmin = HttpContext.Session.GetString("IsAdmin");

            if (string.IsNullOrWhiteSpace(token) || isAdmin != "true")
            {
                return RedirectToAction("Login", "Auth", new { area = "Admin" });
            }

            return View();
        }
    }
}
