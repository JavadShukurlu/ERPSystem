using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }
    }
}
