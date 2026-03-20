using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniSchoolSystem.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin, Teacher")]
    public class TeacherController : Controller
    {
        private readonly UserManager<UserDb> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;
        private readonly ICourseService _courseService;
        private readonly IFileService _fileService;
        private readonly IUserService _UserService;
        private double totalRevenue;

        public TeacherController(UserManager<UserDb> userManager, RoleManager<IdentityRole> roleManager, AppDbContext dbContext, ICourseService courseService, IFileService fileService, IUserService userService, double totalRevenue)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _courseService = courseService;
            _fileService = fileService;
            _UserService = userService;
            this.totalRevenue = totalRevenue;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Get the current Teacher's Integer ID (we did this in the last step)
            var loginId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var teacher = await _dbContext.DbTeacher.FirstOrDefaultAsync(t => t.TeacherId == loginId);

            if (teacher != null)
            {
                // 2. Count all students WHERE the TeacherId matches this teacher
                // (Check your Student model to see if the column is 'TeacherId')
                var totalStudents = await _dbContext.DbStudents
                    .AsNoTracking()
                    .CountAsync(m => m.Id == teacher.Id);

                ViewBag.StudentCount = totalStudents;
            }
            return View();
        }
        [Authorize(Roles = "Teacher")]

        // 1. SEE MY STUFF (Dashboard)
        public async Task<IActionResult> MyCourses(int Id)
        {
            // 1. Get the STRING Identity ID 
            var loginId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. Find the Teacher Profile to get the INT ID (e.g., 5)
            var teacher = _dbContext.DbTeacher
                .FirstOrDefault(t => t.TeacherId == loginId);

            if (teacher == null)
            {
                return Unauthorized("You do not have a Teacher profile setup.");
            }

            var Courses = await _dbContext.DbCourse.Include(m => m.CourseModules).ThenInclude(m => m.Lessons).ThenInclude(o => o.LessonContents).AsNoTracking().Where(m => m.Id == Id && m.CourseUserbID == teacher.TeacherId).ToListAsync();
            return View(Courses);


        }
        // 2. EDIT MY MODULE
        [HttpPost]
        public async Task<IActionResult> EditModule(EditCourseModuleDTO dto)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Teacher = await _dbContext.DbTeacher.FirstOrDefaultAsync(m => m.TeacherId == user);
            if (Teacher == null) return RedirectToAction(nameof(Index));

            var module = await _dbContext.DbModules.FindAsync(dto.CourseId);
            if (module == null) return Unauthorized("CourseModule Doesnt Belong to you");

            // SECURITY CHECK: Make sure Teacher A isn't editing Teacher B's work
            if (module.TeacherId != Teacher.Id) return Unauthorized("This is not your module!");

            module.Title = dto.Title ?? "null";
            module.CreatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(MyCourses));
        }
       // =====================================================//
       //                
    }

}
