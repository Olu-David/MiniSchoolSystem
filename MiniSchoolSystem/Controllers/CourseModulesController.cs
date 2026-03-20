using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Models;

namespace MiniSchoolSystem.Controllers
{
    public class CourseModulesController : Controller
    {
        private readonly UserManager<UserDb> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;
        private readonly IFileService _fileService;

        public CourseModulesController(UserManager<UserDb> userManager, RoleManager<IdentityRole> roleManager, AppDbContext dbContext, IFileService fileService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            return View();
        }
    
    

            //-------------------------------------------CREATECOURSEMODULE---------------------------------------------------------------//

           [Authorize(Roles = "SuperAdmin, Teacher")]
        public async Task<IActionResult> CreateCourseModules()
        {
            var UserId = _userManager.GetUserId(User);
            var Teacher = await _dbContext.DbTeacher.FirstOrDefaultAsync(m => m.TeacherId == UserId);
            if (Teacher == null)
            {
                return BadRequest();
            }
            return View();
        }
        [Authorize(Roles = "SuperAdmin, Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourseModules(CreateCourseModulesViewDTO model)
        {
            //1---GetUserIdentity
            var UserId = _userManager.GetUserId(User);
            var Teacher = await _dbContext.DbTeacher.Include(m => m.TeacherSections).FirstOrDefaultAsync(m => m.TeacherId == UserId);
            if (Teacher == null)
            {
                return BadRequest();
            }
            //2---Validation Check
            if (!ModelState.IsValid)
            {
                ViewBag.CourseSection = Enum.GetValues(typeof(Sections)).Cast<Sections>().ToList();
                ViewBag.CourseSelection = await _dbContext.DbCourse.Where(m => m.Id == model.CourseId).ToListAsync();
                return View(model);
            }
            // 3. Check if CourseModule exists (Fix: Id check)
            bool hasModule = await _dbContext.DbModules.AnyAsync(m => m.Title == model.Title && m.TeacherId == Teacher.Id);
            if (hasModule)
            {
                ModelState.AddModelError("", "You have already created a course with this title.");
                return View(model);
            }
            var Course = await _dbContext.DbCourse.Include(m => m.CourseModules).FirstOrDefaultAsync(m => m.TeacherID == Teacher.Id && m.Id == model.CourseId);
            if (Course == null)

            {
                return NoContent();
            }

            // 4. Section Validation 
            var teacherSections = Teacher.TeacherSections.Select(m => m.TSection).ToList();
            if (!teacherSections.Contains(model.SelectedSection))
            {
                TempData["Error"] = "You are not authorized to create Modules in this section.";
                return RedirectToAction(nameof(Index));
            }

            // 5. Create and Save
            var newModules = new CourseModule
            {
                Title = model.Title??"Null",
                TeacherId = Teacher.Id,
                CourseSections = model.SelectedSection,
                CourseId = model.CourseId

            };

            _dbContext.DbModules.Add(newModules);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = "Course created successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
