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
