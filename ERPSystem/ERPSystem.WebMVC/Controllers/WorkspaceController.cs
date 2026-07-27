using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Controllers
{
    public class WorkspaceController : Controller
    {
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            return View();
        }

        public IActionResult Apps()
        {
            return View();
        }
    }
}
