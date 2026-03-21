using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Models;
using System.Security.Claims;

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
        [HttpGet]
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
                Title = model.Title ?? "Null",
                TeacherId = Teacher.Id,
                CourseId = model.CourseId,
                isArchiveModule = false,
                DeletedAt = null,
                IsDeleted = false,
                ArchiveAt =null,
                CreatedAt=DateTime.UtcNow

            }; 


            _dbContext.DbModules.Add(newModules);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = "Course created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditCourseModules(EditCourseModuleDTO model, int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Teacher = await _dbContext.DbTeacher.Include(s => s.TeacherSections).FirstOrDefaultAsync(t => t.TeacherId == userId);
            if (Teacher == null) return RedirectToAction("Login", "Account");

            var ExistingCourseModules = await _dbContext.DbModules.AnyAsync(m => m.Id == id && m.TeacherId == Teacher.Id);
            if (!ExistingCourseModules)
            {
                TempData["Error"] = "CourseModuoes Does Exists or Teacher Does not belong to the Selected Section";
                return RedirectToAction(nameof(Index));
            }

            var Modules = await _dbContext.DbModules.FirstOrDefaultAsync(m => m.Id == id);
            if (Modules == null)
            {
                TempData["Error"] = "Modules not found";
                return RedirectToAction(nameof(Index));
            }
            var EditModules = new EditCourseModuleDTO
            {
                Title = Modules.Title,
                CreatedAt = Modules.CreatedAt,

            };

            return View(Modules);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourseModules(EditCourseModuleDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Teacher = await _dbContext.DbTeacher.Include(s => s.TeacherSections).FirstOrDefaultAsync(t => t.TeacherId == userId);
            if (Teacher == null) return RedirectToAction("Login", "Account");

            var ExistingCourseModules = await _dbContext.DbModules.AnyAsync(m => m.Id == model.CourseId && m.TeacherId == Teacher.Id);
            if (!ExistingCourseModules)
            {
                TempData["Error"] = "CourseModules Does Exists or Teacher Does not belong to the Selected Section";
                return RedirectToAction(nameof(Index));
            }
            var Modules = await _dbContext.DbModules.FirstOrDefaultAsync(m => m.Id == model.CourseId);
            if (Modules == null)
            {
                TempData["Error"] = "Modules not found";
                return RedirectToAction(nameof(Index));
            }

            Modules.Title = model.Title ?? "Null";
            Modules.CreatedAt = DateTime.UtcNow;


            _dbContext.DbModules.Update(Modules);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = Modules.Id });
        }

        [HttpGet]
        public IActionResult DeleteCourse()
        { return View(); }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourseModule(int Id)
        {
            var UserID = _userManager.GetUserId(User);
            var Teacher = _dbContext.DbTeacher.FirstOrDefault(m => m.TeacherId == UserID);
            if (Teacher == null) return RedirectToAction("Login", "Account");

            var ModuleExists = await _dbContext.DbModules.AnyAsync(m => m.Id == Id);
            if (!ModuleExists)
            {
                TempData["Error"] = "Modules not Found";
                return RedirectToAction(nameof(Index));
            }
            var ExistingModule = await _dbContext.DbModules.Include(m => m.Lessons).ThenInclude(i => i.LessonContents).FirstOrDefaultAsync(m => m.Id == Id && m.TeacherId == Teacher.Id);
            if (ExistingModule == null)
            {
                TempData["Error"] = "Modules Doesnt Exist";
                return RedirectToAction(nameof(Index));
            }

            ExistingModule.IsDeleted = true;
            ExistingModule.DeletedAt = DateTime.UtcNow;
            if (ExistingModule.Lessons != null && ExistingModule.Lessons.Any())
            {
                foreach (var lesson in ExistingModule.Lessons)
                {
                    lesson.IsDeleted = true;
                    lesson.DeletedAt = DateTime.UtcNow;

                }
            }


            await _dbContext.SaveChangesAsync();
            TempData["information"] = "Course Modules Deleted, User can restore within 30-Days";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> ManageDeletedModules(int Id)
        {
            var UserID = _userManager.GetUserId(User);
            var Teacher = _dbContext.DbTeacher.FirstOrDefault(m => m.TeacherId == UserID);
            if (Teacher == null) return RedirectToAction("Login", "Account");


            var ExistingModule = await _dbContext.DbModules.Include(m => m.Lessons).ThenInclude(i => i.LessonContents).Where(m => m.Id == Id && m.TeacherId == Teacher.Id && m.IsDeleted).OrderByDescending(m => m.DeletedAt).ToListAsync();
            if (ExistingModule == null)
            {
                TempData["Error"] = "Modules Doesnt Exist";
                return RedirectToAction(nameof(Index));
            }
            return View(ExistingModule);
        }
        [HttpGet]
        public async Task<IActionResult> RestoreModule(int Id)
        {
            var UserID = _userManager.GetUserId(User);
            var Teacher = _dbContext.DbTeacher.FirstOrDefault(m => m.TeacherId == UserID);
            if (Teacher == null) return RedirectToAction("Login", "Account");


            var ExistingModule = await _dbContext.DbModules.Include(m => m.Lessons).ThenInclude(i => i.LessonContents).FirstOrDefaultAsync(m => m.Id == Id && m.TeacherId == Teacher.Id);
            if (ExistingModule == null)
            {
                TempData["Error"] = "Modules Doesnt Exist";
                return RedirectToAction(nameof(Index));
            }
            if (ExistingModule.DeletedAt.HasValue)
            {
                var DaySinceDeleted = (DateTime.UtcNow - ExistingModule.DeletedAt.Value).TotalDays;

                if (DaySinceDeleted > 30)
                {
                    TempData["Error"] = "Unable to Deleted Item, Item Exceeded 30days";
                    return RedirectToAction(nameof(DeleteCourse));
                }
                ExistingModule.IsDeleted = false;
                ExistingModule.DeletedAt = null;

                foreach (var Lessson in ExistingModule.Lessons)
                {
                    Lessson.IsDeleted = false;
                    Lessson.DeletedAt = null;
                }
                await _dbContext.SaveChangesAsync();
                TempData["Success"] = "Module and its lessons have been restored successfully!";
            }

            // ✅ IMPORTANT: You must return a view or redirect!
            return RedirectToAction(nameof(Index), new { id = ExistingModule.Id });
        }

        public async Task<IActionResult> DeletedPermanently(int Id)
        {
            var UserID = _userManager.GetUserId(User);
            var Teacher = _dbContext.DbTeacher.FirstOrDefault(m => m.TeacherId == UserID);
            if (Teacher == null) return RedirectToAction("Login", "Account");


            var ExistingModule = await _dbContext.DbModules.Include(m => m.Lessons).ThenInclude(i => i.LessonContents).FirstOrDefaultAsync(m => m.Id == Id && m.TeacherId == Teacher.Id && m.IsDeleted);
            if (ExistingModule == null)
            {
                TempData["Error"] = "Modules Doesnt Exist";
                return RedirectToAction(nameof(Index));
            }
            if (ExistingModule.DeletedAt.HasValue)
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-30);


                if (ExistingModule.DeletedAt.Value < cutoffDate)
                {
                    TempData["Error"] = "This module is too old to be restored (exceeded 30 days).";
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index), new { id = ExistingModule.Id });
            // 4. If we reach here, it's safe to restore!
            //foreach (var lesson in ExistingModule.Lessons)
            //{
            //    lesson.IsDeleted = false;
            //    lesson.DeletedAt = null;

            //    //// Restore the contents inside each lesson
            //    //if (lesson.LessonContents != null)
            //    //{
            //    //    foreach (var content in lesson.LessonContents)
            //    //    {
            //    //        content.IsDeleted = false;
            //    //        content.DeletedAt = null;
            //    //    }
            //    //}
            //}
        }
        public async Task<IActionResult> ArchivedModule(int Id)
        {
            var UserID = _userManager.GetUserId(User);
            var Teacher = _dbContext.DbTeacher.FirstOrDefault(m => m.TeacherId == UserID);
            if (Teacher == null) return RedirectToAction("Login", "Account");


            var ExistingModule = await _dbContext.DbModules.Include(m => m.Lessons).ThenInclude(i => i.LessonContents).FirstOrDefaultAsync(m => m.Id == Id && m.TeacherId == Teacher.Id);
            if (ExistingModule == null)
            {
                TempData["Error"] = "Modules Doesnt Exist";
                return RedirectToAction(nameof(Index));
            }
            ExistingModule.isArchiveModule = true;
            ExistingModule.ArchiveAt = DateTime.UtcNow;
            TempData["Info"] = "Module Archived Successful";
            return RedirectToAction(nameof(Index));
        }
    }
}

