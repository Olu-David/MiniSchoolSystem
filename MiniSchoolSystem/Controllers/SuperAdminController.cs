using Microsoft.AspNetCore.Mvc;

namespace MiniSchoolSystem.Controllers
{
    public class SuperAdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
