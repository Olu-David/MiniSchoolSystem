using Microsoft.AspNetCore.Mvc;

namespace MiniSchoolSystem.Controllers
{
    public class AssignmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
