using Microsoft.AspNetCore.Mvc;

namespace MiniSchoolSystem.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
