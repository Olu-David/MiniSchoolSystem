using Microsoft.AspNetCore.Mvc;

namespace MiniSchoolSystem.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
