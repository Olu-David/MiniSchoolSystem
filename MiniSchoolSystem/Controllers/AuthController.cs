using Microsoft.AspNetCore.Mvc;

namespace MiniSchoolSystem.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
