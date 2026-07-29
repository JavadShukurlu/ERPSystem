using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
