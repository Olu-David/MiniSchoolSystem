using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Implementation.Services;
using MiniSchoolSystem.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniSchoolSystem.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly UserManager<UserDb> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;
        private readonly ICourseService _courseService;
        private readonly IFileService _fileService;
        private readonly IUserService _UserService;
        private double totalRevenue;

        public SuperAdminController(UserManager<UserDb> userManager, RoleManager<IdentityRole> roleManager, AppDbContext dbContext, ICourseService courseService, IFileService fileService, IUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _courseService = courseService;
            _fileService = fileService;
            _UserService = userService;
        }

        public IActionResult Index()
        {
          
            return View();
        }
      

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(string userId, bool isLocked)
        {
            var (success, message) = await _UserService.SetUserLockoutAsync(userId, isLocked);

            if (success)
            {
                TempData["success"] = message;
            }
            else
            {
                TempData["error"] = message;
            }

            return RedirectToAction("ManageUsers"); // Or wherever your user list is
        }
        // GET: List all Teachers and Admins
        public async Task<IActionResult> ManageStaff()
        {
            var staff = await _userManager.GetUsersInRoleAsync("Teacher, Admin");
            return View(staff);
        }

        public IActionResult PromoteToAdmin()
        {
            return View();
        }
        // POST: Promote a User to Admin
        [HttpPost]
        public async Task<IActionResult> PromoteToAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            return RedirectToAction(nameof(ManageStaff));
        }

        // GET: System Health / Statistics
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Dashboard()
        {
            // 1. Get Totals (No ID needed - we want EVERYONE)
            var studentCount = await _dbContext.DbStudents.AsNoTracking().CountAsync();
            var teacherCount = await _dbContext.DbTeacher.AsNoTracking().CountAsync();

            // 2. Filtered Count (Active Courses only)
            var courseCount = await _dbContext.DbCourse
                .AsNoTracking()
                .CountAsync(c => !c.IsDeleted);

            // 3. Calculation (Total Revenue)
            // Assuming your Course has a 'Price' and Students have an 'IsPaid' status
           

            // 4. Pass data to the View using a ViewModel or ViewBag
            var stats = new AdminDashboardViewModel
            {
                TotalStudents = studentCount,
                TotalTeachers = teacherCount,
                TotalCourses = courseCount,
                TotalRevenue = totalRevenue
            };

            return View(stats);
        }
    }
}
