using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
