using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MiniSchoolSystem.Controllers
{
    public class QuizController1 : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            TempData["info"] = "Quiz Coming Soon, Make Una calm down 😄";
            return View();
        }
    }
}
