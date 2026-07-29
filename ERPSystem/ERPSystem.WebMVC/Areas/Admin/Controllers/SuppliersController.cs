using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SuppliersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
