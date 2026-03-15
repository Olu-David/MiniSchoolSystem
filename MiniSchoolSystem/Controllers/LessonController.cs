using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Models;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniSchoolSystem.Controllers
{


    public class LessonController : Controller
    {
        private readonly UserManager<UserDb> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<LessonController> _logger;
        private readonly IFileService _fileService;



        public LessonController(UserManager<UserDb> userManager, RoleManager<IdentityRole> roleManager, AppDbContext dbContext, ILogger<LessonController> logger, IFileService fileService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _logger = logger;
            _fileService = fileService;
        }
        [HttpGet]
        public  IActionResult Index()
        {
            return View();
        }
        [HttpGet]
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
            // 3. Check if Course exists (Fix: Id check)
            bool hasCourse = await _dbContext.DbCourse.AnyAsync(m => m.Title == model.Title && m.TeacherID == Teacher.Id);
            if (hasCourse)
            {
                ModelState.AddModelError("", "You have already created a course with this title.");
                return View(model);
            }

            // 4. Section Validation 
            var teacherSections = Teacher.TeacherSections.Select(m => m.TSection).ToList();
            if (!teacherSections.Contains(model.SelectedSection))
            {
                TempData["Error"] = "You are not authorized to create courses in this section.";
                return RedirectToAction(nameof(Index));
            }

            // 5. Create and Save
            var newCourse = new Course
            {
                Title = model.Title,
                Slug = $"{Guid.NewGuid()}-1",
                CreatedAt = DateTime.UtcNow,
                CourseSections = model.SelectedSection,
                TeacherID = Teacher.Id
            };

            _dbContext.DbCourse.Add(newCourse);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = "Course created successfully!";
            return RedirectToAction(nameof(CreateCourseModules));
        }
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpGet]
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> EditCourse(EditCourseDTO dto, int Id)
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
            var Teacher = await _dbContext.DbTeacher.Include(m => m.TeacherSections).FirstOrDefaultAsync(m => m.TeacherId == Userid);
            if (Teacher == null)
            {
                return Unauthorized();
            }
            //Filtering Teacher by its Default Section
            var TeacherSection = Teacher.TeacherSections.Select(t => t.TSection);
            if(!TeacherSection.Contains(dto.SelectedSection))
            {
                return BadRequest("Teacher Doesnt Belong to the Selected Section");
            }
            //Getting the List of existing Courses
            var ExistingCourse = await _dbContext.DbCourse.FirstOrDefaultAsync(m=>m.Id==Id);
         
            if(ExistingCourse==null)
            {
                return NotFound("Course not Available");
            }
            // SECURITY CHECK: Ensure this teacher owns the course they are editing
            if (ExistingCourse.TeacherID != Teacher.Id && !User.IsInRole("SuperAdmin"))
            {
                return Forbid("You do not have permission to edit this course.");
            }
           
            //  UPDATE the existing object (Don't create a 'new Course')
            ExistingCourse.Title = dto.Title;
            ExistingCourse.CourseSections = dto.SelectedSection;
            ExistingCourse.LastModifiedAt = DateTime.UtcNow;

            // Pro-Tip: Only update the Slug if the title changed
            ExistingCourse.Slug = $"{dto.Title?.ToLower().Replace(" ", "-")}-{Id}";

            // 7. Save Changes
            try
            {
                // EF is already tracking 'existingCourse', so .Update() is often optional 
                // but good for clarity.
                _dbContext.DbCourse.Update(ExistingCourse);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again.");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles ="SuperAdmin, Admin")]
        [HttpGet]
        public  IActionResult ArchiveCourse()
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
        public async Task<IActionResult> DeleteCourse(int Id)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (UserId == null)
            {
                return Unauthorized();
            }
            if (!User.IsInRole("Admin"))
                return Unauthorized("Only SuperAdmin and Admin can delete courses.");
            var ExistingCourse = await _dbContext.DbCourse.Include(m => m.CourseModules).ThenInclude(m => m.Lessons).ThenInclude(m => m.LessonContent).FirstOrDefaultAsync(c => c.Id == Id);
            if (ExistingCourse == null)
            {
                return NotFound("Lesson Unaavailable");
            }
            // Schedule deletion
            ExistingCourse.IsDeleted = true;
            ExistingCourse.DeletedAt = DateTime.Now;

            // Also mark lessons as deleted
            foreach (var module in ExistingCourse.CourseModules)
                foreach (var lesson in module.Lessons)
                {
                    lesson.IsDeleted = true;
                    lesson.DeletedAt = DateTime.UtcNow;
                }

            await _dbContext.SaveChangesAsync();
            return Ok("Course scheduled for deletion (3-day window for SuperAdmin cancel).");
        }

        // Restore Course (SuperAdmin only)
        [HttpPut]
        public async Task<IActionResult> RestoreCourse(int id)
        {
            if (!User.IsInRole("SuperAdmin")) return Unauthorized();

            var course = await _dbContext.DbCourse
                .Include(c => c.CourseModules)
                .ThenInclude(m => m.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) return NotFound();
            if (course.DeletedAt.HasValue)
            {
                var daysSinceDeleted = (DateTime.Now - course.DeletedAt.Value).TotalDays;
                if (daysSinceDeleted > 30) return BadRequest("Cannot cancel, 30-days period expired");

                course.IsDeleted = false;
                course.DeletedAt = null;

                foreach (var module in course.CourseModules)
                    foreach (var lesson in module.Lessons)
                    {
                        lesson.IsDeleted = false;
                        lesson.DeletedAt = null;
                    }

                await _dbContext.SaveChangesAsync();
                return Ok("Course deletion cancelled successfully");
            }
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
                Title = model.Title,
                TeacherId = Teacher.Id,
                CourseSections = model.SelectedSection,
                CourseId = model.CourseId

            };

            _dbContext.DbModules.Add(newModules);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = "Course created successfully!";
            return RedirectToAction(nameof(Index));
        }

        //-------------------------------------------CREATELESSON---------------------------------------------------------------//
       
        [Authorize(Roles = "SuperAdmin, Teacher")]
        [HttpGet]
        public async Task<IActionResult> CreateLesson()
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
        public async Task<IActionResult> CreateLesson(CreateLessonViewDTO model, CreateLessonContentDTO dto)
        {
            var userId = _userManager.GetUserId(User);

            // 1. Get Teacher and their Sections
            var teacher = await _dbContext.DbTeacher
                .Include(m => m.TeacherSections)
                .FirstOrDefaultAsync(m => m.TeacherId == userId);

            if (teacher == null) return NotFound();

            if (!ModelState.IsValid) return View(model);

            // 2. Check if a lesson with this ID already exists (Security check)
            var alreadyExists = await _dbContext.DbLesson.AnyAsync(m => m.Id == model.Id&&m.TeacherId==teacher.Id);
            if (alreadyExists)
            {
                return BadRequest("Lesson already exists.");
            }

            // 3. Authorization: Check if teacher owns this section
            var teacherSections = teacher.TeacherSections.Select(m => m.TSection).ToList();
            if (!teacherSections.Contains(model.LessonSection))
            {
                TempData["Error"] = "You are not authorized for this section.";
                return RedirectToAction(nameof(Index));
            }

            // 4. Handle the file upload from the 'dto' parameter
            string? attachmentUrl = null;
            if (dto.file != null)
            {
                attachmentUrl = await _fileService.SaveFileAsync(dto.file, "LessonFiles");
            }

            // 5. Create the NEW Lesson (Mapping from DTOs to Database Entities)
            var newLesson = new Lesson
            {
                Title = model.Title,
                Description = model.Description,
                TeacherId = teacher.Id, // Link it to the fetched teacher
                CreatedAt = DateTime.UtcNow,
                // Create the child content record
                LessonContent = new List<LessonContent>
        {
            new LessonContent
            {
                Content = dto.Content, // Data from 'dto'
                FileUrl = attachmentUrl,   // The saved file path
                CreatedAt = DateTime.UtcNow
            }
        }
            };

            // 6. Save to DB
            _dbContext.DbLesson.Add(newLesson);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // Archive Lesson (Teacher)
        [HttpPut("archive/{id}")]
        public async Task<IActionResult> ArchiveLesson(int id)
        {
            if (!User.IsInRole("Teacher")) return Unauthorized();

            var lesson = await _dbContext.DbLesson.FirstOrDefaultAsync(l => l.Id == id);
            if (lesson == null) return NotFound();

            lesson.IsArchived = true;
            await _dbContext.SaveChangesAsync();
            return Ok("Lesson archived successfully");
        }

        // Soft Delete Lesson
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var lesson = await _dbContext.DbLesson.FirstOrDefaultAsync(l => l.Id == id);
            if (lesson == null)
                return NotFound("Lesson not found");

            if (User.IsInRole("Teacher"))
            {
                // Teacher schedules deletion
                lesson.IsDeleted = true;
                lesson.DeletedAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
                return Ok("Lesson scheduled for deletion (SuperAdmin can cancel within 3 days).");
            }

            if (User.IsInRole("SuperAdmin"))
            {
                _dbContext.DbLesson.Remove(lesson);
                await _dbContext.SaveChangesAsync();
                return Ok("Lesson permanently deleted.");
            }

            return Unauthorized();
        }

        // Restore Lesson (Admin only)
        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreLesson(int id)
        {
            if (!User.IsInRole("SuperAdmin")) return Unauthorized();

            var lesson = await _dbContext.DbLesson.FirstOrDefaultAsync(l => l.Id == id);
            if (lesson == null) return NotFound();
            if (!lesson.IsDeleted) return BadRequest("Lesson is not deleted");

            if (lesson.DeletedAt.HasValue)
            {
                var daysSinceDeleted = (DateTime.Now - lesson.DeletedAt.Value).TotalDays;
                if (daysSinceDeleted > 30) return BadRequest("Cannot cancel, 3-day period expired");

                lesson.IsDeleted = false;
                lesson.DeletedAt = null;
                await _dbContext.SaveChangesAsync();
               
            }
            return Ok("Lesson deletion cancelled successfully");
        }
        // Get Active Lessons
        [HttpGet]
        public async Task<IActionResult> GetLessons()
        {
            var lessons = await _dbContext.DbLesson
                .Where(l => !l.IsDeleted && !l.IsArchived)
                .ToListAsync();
            return Ok(lessons);
        }

        // Permanent Delete after 3 days (can run in background)
        public async Task DeleteExpiredLessons()
        {
            var expired = await _dbContext.DbLesson
                .Where(l => l.IsDeleted && l.DeletedAt <= DateTime.Now.AddDays(-3))
                .ToListAsync();
            _dbContext.DbLesson.RemoveRange(expired);
            await _dbContext.SaveChangesAsync();
        }
    }
}
