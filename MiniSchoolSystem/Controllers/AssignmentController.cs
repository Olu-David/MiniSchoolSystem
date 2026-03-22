using Microsoft.AspNetCore.Mvc;

namespace MiniSchoolSystem.Controllers
{
    public class AssignmentController : Controller
    {
        public IActionResult Index()
        {
       
        
            TempData["info"] = "Quiz Coming Soon, Make Una calm down 😄";
            return View();
        }
    }
    }
}
