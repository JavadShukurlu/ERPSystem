using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.WebMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuditLogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
