using Microsoft.AspNetCore.Mvc;
using System;

namespace MiniSchoolSystem.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GoogleLogin()
        {
            return Content("Google Login Coming Soon, Stay Jiggy");
        }
        public IActionResult AppleLogin()
        { 
            return Content("Apple Login Coming Soon, Stay Jiggy");
        }
        public IActionResult YahooLogin()
        {
            return Content("Yahoo Login Coming Soon, Stay Jiggy");
        }
    }
}
 