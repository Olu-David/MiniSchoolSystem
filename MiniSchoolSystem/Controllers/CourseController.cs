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
    public class CourseController : Controller
    {
        private readonly UserManager<UserDb> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;
        private readonly ICourseService _courseService;
        private readonly IFileService _fileService;

        public CourseController(UserManager<UserDb> userManager, RoleManager<IdentityRole> roleManager, AppDbContext dbContext, ICourseService courseService, IFileService fileService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _courseService = courseService;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }
            
        //-------------------------------------------CREATECOURSE---------------------------------------------------------------//
        [HttpGet]
        [Authorize(Roles = "SuperAdmin, Teacher")]
        public async Task<IActionResult> CreateCourse()
        {
            var UserId = _userManager.GetUserId(User);
            var Teacher = await _dbContext.DbTeacher.FirstOrDefaultAsync(m => m.TeacherId == UserId);
            if (Teacher == null)
            {
                return BadRequest();
            }
            return View();
        }
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(CreateCourseViewDTO model)
        {
            //1---GetUserIdentity
            var UserId = _userManager.GetUserId(User);
            if(UserId==null) return BadRequest();
            var Teacher = await _dbContext.DbTeacher.Include(m => m.TeacherSections).FirstOrDefaultAsync(m => m.TeacherId == UserId);
            if (Teacher == null)
            {
                return BadRequest();
            }
            //2---Validation Check
            if (!ModelState.IsValid)
            {
                ViewBag.CourseSection = Enum.GetValues(typeof(Sections)).Cast<Sections>().ToList();
                return View(model);
            }
            var (success, message, courseId) = await _courseService.CreateCourseAsync(model, UserId) ;

            if (!success)
            {
                ModelState.AddModelError("", "There was an error saving the course. Please try again.");
                ViewBag.CourseSection = Enum.GetValues(typeof(Sections)).Cast<Sections>().ToList();
                return View(model);
            }

            
            return RedirectToAction("CreateCourseModules" ,"CourseModules", new { id = courseId });
        }
       
       
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
       
        public async Task<IActionResult> EditCourse(int Id)
        {
            //Identification
            var Userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Userid == null)
            {
                return Unauthorized("User Not Authorized to Access this Page");

            }

            var Teacher = await _dbContext.DbTeacher.Include(m => m.TeacherSections).FirstOrDefaultAsync(m => m.TeacherId == Userid);
            if (Teacher == null)
            {
                return Unauthorized();
            }
            //Deep Search(Family Tree)
            var Course = await _dbContext.DbCourse.FirstOrDefaultAsync(m => m.Id == Id && m.TeacherID == Teacher.Id);
            if (Course == null)
            {
                return NotFound("Course not Available");
            }
            var DisplayCourses = new EditCourseDTO
            {
                Title = Course.Title,
                LastModifiedAt = Course.LastModifiedAt,
                Id = Course.Id


            };
            return View(DisplayCourses);
        }
        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// 
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(EditCourseDTO dto, string Id)
        {
            //Identification
            var Userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Userid == null)
            {
                return Unauthorized("User Not Authorized to Access this Page");

            }
            if (!ModelState.IsValid)
            {
                ViewBag.CourseSection = Enum.GetValues(typeof(Sections)).Cast<Sections>().ToList();
                return View(dto);
            }
            var (Success, Message) = await _courseService.EditCourseAsync(dto, Userid);
            if(!Success)
            {
                ModelState.AddModelError("", "Edit Course is Unsuccessfully");
                ViewBag.CourseSection = Enum.GetValues(typeof(Sections)).Cast<Sections>().ToList();
                return View(dto);   

            }
            return RedirectToAction(nameof(Details), new {id=dto.Id});  
        }


        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        public IActionResult ArchiveCourse()
        {
            var Userid = _userManager.GetUserId(User);
            if (Userid == null)
            {
                return BadRequest();
            }

            if (!User.IsInRole("SuperAdmin"))
            {
                return Unauthorized();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveCourse(int id)
        {
            if (!User.IsInRole("SuperAdmin")) return Unauthorized();

            var course = await _dbContext.DbCourse.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) return NotFound();

            course.IsArchived = true;
            await _dbContext.SaveChangesAsync();
            return Ok("Course archived successfully");
        }


        [Authorize(Roles = " Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int CourseId, string Id )
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserId == null)
            {
                return Unauthorized();
            }
            if (!User.IsInRole("Admin"))
                return Unauthorized("Only SuperAdmin and Admin can delete courses.");
            var (success, message) = await _courseService.DeleteCourseAsync(Id, CourseId);
            if(!success)
            {
                ModelState.AddModelError(string.Empty, "Deletion not completed");
            }

            return RedirectToAction(nameof(Index));

        }

        // Restore Course (SuperAdmin only)
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreCourse(int CourseId, string id)
        {
            // 1. Validation: Ensure we have the data
            if (CourseId == 0 || string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid Course or User ID.");
            }

            // 2. Call the Service
            var (success, message) = await _courseService.RestoreCourseAsync(CourseId, id);

            if (!success)
            {
                // If it fails, we stay on the current page and show the error
                TempData["Error"] = message;

                // Option A: Redirect back to where they came from
                return RedirectToAction(nameof(Index));

                // Option B: If you have a specific "Restore" confirmation page:
                // return View("RestoreConfirmation", CourseId);
            }

            // 3. Success logic
            TempData["info"] = "Restoration Successful!";

            // Redirect to the Index (Trash Bin or Course List)
            return RedirectToAction(nameof(Index));
        }
        // Get Active Courses
        [HttpGet("all")]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _dbContext.DbCourse
                .Where(c => !c.IsDeleted && !c.IsArchived)
                .ToListAsync();
            return Ok(courses);
        }

        // Permanent Delete after 30 days (background)
        public async Task DeleteExpiredCourses()
        {
            var expired = await _dbContext.DbCourse
                .Where(c => c.IsDeleted && c.DeletedAt <= DateTime.Now.AddDays(-30))
                .Include(c => c.CourseModules)
                .ThenInclude(m => m.Lessons)
                .ToListAsync();

            foreach (var course in expired)
                foreach (var module in course.CourseModules)
                    _dbContext.DbLesson.RemoveRange(module.Lessons);

            _dbContext.DbCourse.RemoveRange(expired);
            await _dbContext.SaveChangesAsync();
        }
        // GET: Course/Duplicate/5
        [Authorize(Roles = "SuperAdmin, Admin, Teacher")]
        [HttpGet]
        public async Task<IActionResult> Duplicate(int id)
        {
            var course = await _dbContext.DbCourse
                .Select(c => new EditCourseDTO
                {
                    Id = c.Id,
                    Title = c.Title
                })
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null) return NotFound("Course not found.");

            return View(course);
        }

        // POST: Course/Duplicate/5
        [Authorize(Roles = "SuperAdmin, Admin, Teacher")]
        [HttpPost]
        [ActionName("Duplicate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuplicateConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var teacher = await _dbContext.DbTeacher.FirstOrDefaultAsync(t => t.TeacherId == userId);

            // Fetch the full tree: Course -> Modules -> Lessons
            var oldCourse = await _dbContext.DbCourse
                .Include(c => c.CourseModules)
                    .ThenInclude(m => m.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (oldCourse == null) return NotFound();

            var clonedCourse = new Course
            {
                Title = oldCourse.Title + " (Copy)",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                // Clone Modules
                CourseModules = oldCourse.CourseModules.Where(m => !m.IsDeleted).Select(m => new CourseModule
                {
                    Title = m.Title,
                    // If a teacher is doing the cloning, they 'own' the new modules
                    TeacherId = teacher != null ? teacher.Id : m.TeacherId,
                    Order = m.Order,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    // Clone Lessons inside the Modules
                    Lessons = m.Lessons.Where(l => !l.IsDeleted).Select(l => new Lesson
                    {
                        Title = l.Title,
                        Description = l.Description,
                        CreatedAt = DateTime.UtcNow
                    }).ToList()
                }).ToList()
            };

            _dbContext.DbCourse.Add(clonedCourse);
            await _dbContext.SaveChangesAsync();

            TempData["info"] = "Course duplicated successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
