using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Models;

namespace MiniSchoolSystem.Controllers
{
    public class LessonController : Controller
    {
        private readonly UserManager<UserDb> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<LessonController> _logger;
        private readonly IFileService _fileService;

        public LessonController(
            UserManager<UserDb> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext dbContext,
            ILogger<LessonController> logger,
            IFileService fileService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _logger = logger;
            _fileService = fileService;
        }

        // ────────────────────────────────────────────────────────
        //  INDEX
        // ────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // ────────────────────────────────────────────────────────
        //  DETAILS
        // ────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // BUG FIX: original had no 'id' parameter — view had no data
            var lesson = await _dbContext.DbLesson
                .Include(l => l.LessonContent)
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted && !l.IsArchived);

            if (lesson == null) return NotFound();

            ViewBag.Lesson = lesson;
            return View();
        }

        // ────────────────────────────────────────────────────────
        //  GET LESSONS (JSON — used by Index JS fetch)
        // ────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetLessons()
        {
            var lessons = await _dbContext.DbLesson
                .Where(l => !l.IsDeleted && !l.IsArchived)
                .ToListAsync();
            return Ok(lessons);
        }

        // ────────────────────────────────────────────────────────
        //  CREATE LESSON
        // ────────────────────────────────────────────────────────
        [Authorize(Roles = "SuperAdmin, Teacher")]
        [HttpGet]
        public async Task<IActionResult> CreateLesson()
        {
            var userId = _userManager.GetUserId(User);
            var teacher = await _dbContext.DbTeacher.FirstOrDefaultAsync(m => m.TeacherId == userId);

            if (teacher == null)
            {
                TempData["Error"] = "Teacher profile not found.";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        [Authorize(Roles = "SuperAdmin, Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLesson(CreateLessonViewDTO model, CreateLessonContentDTO dto)
        {
            var userId = _userManager.GetUserId(User);
            var teacher = await _dbContext.DbTeacher
                .Include(m => m.TeacherSections)
                .FirstOrDefaultAsync(m => m.TeacherId == userId);

            if (teacher == null) return NotFound();
            if (!ModelState.IsValid) return View(model);

            // Duplicate check
            var alreadyExists = await _dbContext.DbLesson
                .AnyAsync(m => m.Id == model.Id && m.TeacherId == teacher.Id);
            if (alreadyExists)
            {
                ModelState.AddModelError(string.Empty, "A lesson with this ID already exists.");
                return View(model);
                // BUG FIX: was returning BadRequest — should return View with error
            }

            // Section ownership check
            var teacherSections = teacher.TeacherSections.Select(m => m.TSection).ToList();
            if (!teacherSections.Contains(model.LessonSection))
            {
                TempData["Error"] = "You are not authorized to create lessons for this section.";
                return RedirectToAction(nameof(Index));
            }

            // File upload
            string? attachmentUrl = null;
            if (dto.file != null)
                attachmentUrl = await _fileService.SaveFileAsync(dto.file, "LessonFiles");

            var newLesson = new Lesson
            {
                Title = model.Title,
                Description = model.Description,
                TeacherId = teacher.Id,
                CreatedAt = DateTime.UtcNow,
                LessonContent = new List<LessonContent>
                {
                    new LessonContent
                    {
                        Content   = dto.Content,
                        FileUrl   = attachmentUrl,
                        CreatedAt = DateTime.UtcNow
                    }
                }
            };

            _dbContext.DbLesson.Add(newLesson);
            await _dbContext.SaveChangesAsync();

            TempData["info"] = "Lesson created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ────────────────────────────────────────────────────────
        //  EDIT LESSON
        // ────────────────────────────────────────────────────────
        [Authorize(Roles = "SuperAdmin, Teacher")]
        [HttpGet]
        public async Task<IActionResult> EditLesson(int id)
        {
            var userId = _userManager.GetUserId(User);
            var lesson = await _dbContext.DbLesson
                .Include(l => l.LessonContent)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null) return NotFound();

            if (User.IsInRole("Teacher"))
            {
                var teacher = await _dbContext.DbTeacher
                    .FirstOrDefaultAsync(t => t.TeacherId == userId);
                if (teacher == null || lesson.TeacherId != teacher.Id)
                    return Forbid();
            }

            var model = new CreateLessonViewDTO
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Description = lesson.Description,
                LessonSection = lesson.LessonSection,
            };

            var content = lesson.LessonContent?.FirstOrDefault();
            ViewBag.ExistingContent = content?.Content ?? string.Empty;
            ViewBag.ExistingFileUrl = content?.FileUrl ?? string.Empty;
            ViewBag.IsEdit = true;

            return View(model);
        }

        [Authorize(Roles = "SuperAdmin, Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLesson(int id, CreateLessonViewDTO model, CreateLessonContentDTO dto)
        {
            var userId = _userManager.GetUserId(User);
            var lesson = await _dbContext.DbLesson
                .Include(l => l.LessonContent)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null) return NotFound();

            if (User.IsInRole("Teacher"))
            {
                var teacher = await _dbContext.DbTeacher
                    .FirstOrDefaultAsync(t => t.TeacherId == userId);
                if (teacher == null || lesson.TeacherId != teacher.Id)
                    return Forbid();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.IsEdit = true;
                ViewBag.ExistingContent = lesson.LessonContent?.FirstOrDefault()?.Content ?? string.Empty;
                ViewBag.ExistingFileUrl = lesson.LessonContent?.FirstOrDefault()?.FileUrl ?? string.Empty;
                return View(model);
            }

            lesson.Title = model.Title;
            lesson.Description = model.Description;
            lesson.LessonSection = model.LessonSection;

            var existingContent = lesson.LessonContent?.FirstOrDefault();
            if (existingContent != null)
            {
                existingContent.Content = dto.Content;
                if (dto.file != null)
                    existingContent.FileUrl = await _fileService.SaveFileAsync(dto.file, "LessonFiles");
            }
            else
            {
                string? fileUrl = null;
                if (dto.file != null)
                    fileUrl = await _fileService.SaveFileAsync(dto.file, "LessonFiles");

                lesson.LessonContent = new List<LessonContent>
                {
                    new LessonContent
                    {
                        Content   = dto.Content,
                        FileUrl   = fileUrl,
                        CreatedAt = DateTime.UtcNow
                    }
                };
            }

            await _dbContext.SaveChangesAsync();
            TempData["info"] = "Lesson updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ────────────────────────────────────────────────────────
        //  DELETE LESSON
        // ────────────────────────────────────────────────────────
        [Authorize(Roles = "SuperAdmin, Teacher")]
        [HttpGet]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            // GET: just show the confirmation page — do NOT delete anything
            var lesson = await _dbContext.DbLesson
                .Include(l => l.LessonContent)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null) return NotFound();

            ViewBag.Lesson = lesson;
            return View();
        }

        [Authorize(Roles = "SuperAdmin, Teacher")]
        [HttpPost, ActionName("DeleteLesson")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLessonConfirmed(int id)
        {
            // POST: actually perform the delete
            var lesson = await _dbContext.DbLesson
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null) return NotFound();

            if (User.IsInRole("Teacher"))
            {
                // Soft delete — schedule for removal
                lesson.IsDeleted = true;
                lesson.DeletedAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
                TempData["info"] = "Lesson scheduled for deletion. A SuperAdmin can cancel within 3 days.";
            }
            else if (User.IsInRole("SuperAdmin"))
            {
                // Hard delete — permanent
                _dbContext.DbLesson.Remove(lesson);
                await _dbContext.SaveChangesAsync();
                TempData["info"] = "Lesson permanently deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ────────────────────────────────────────────────────────
        //  ARCHIVE LESSON
        // ────────────────────────────────────────────────────────
        [Authorize(Roles = "SuperAdmin, Teacher")]
        [HttpGet]
        public async Task<IActionResult> ArchiveLesson(int id)
        {
            // GET: just show the confirmation page — do NOT archive anything
            var lesson = await _dbContext.DbLesson
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null) return NotFound();

            ViewBag.Lesson = lesson;
            return View();
        }

        [Authorize(Roles = "SuperAdmin, Teacher")]
        [HttpPost, ActionName("ArchiveLesson")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveLessonConfirmed(int id)
        {
            // POST: actually archive the lesson
            var lesson = await _dbContext.DbLesson
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null) return NotFound();

            lesson.IsArchived = true;
            await _dbContext.SaveChangesAsync();

            TempData["info"] = "Lesson archived successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ────────────────────────────────────────────────────────
        //  RESTORE LESSON
        // ────────────────────────────────────────────────────────
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> RestoreLesson(int id)
        {
            // GET: just show the confirmation page — do NOT restore anything
            var lesson = await _dbContext.DbLesson
                .FirstOrDefaultAsync(l => l.Id == id && l.IsDeleted);

            if (lesson == null)
            {
                TempData["Error"] = "Lesson not found or is not scheduled for deletion.";
                return RedirectToAction(nameof(Index));
            }

            var daysGone = (DateTime.Now - (lesson.DeletedAt ?? DateTime.Now)).TotalDays;
            if (daysGone > 30)
            {
                TempData["Error"] = "Cannot restore — the 30-day window has expired.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Lesson = lesson;
            return View(); // just the "are you sure?" page
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ActionName("RestoreLesson")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreLessonConfirmed(int id)
        {
            // POST: actually restore the lesson
            var lesson = await _dbContext.DbLesson
                .FirstOrDefaultAsync(l => l.Id == id && l.IsDeleted);

            if (lesson == null)
            {
                TempData["Error"] = "Lesson not found or is not scheduled for deletion.";
                return RedirectToAction(nameof(Index));
            }

            var daysGone = (DateTime.Now - (lesson.DeletedAt ?? DateTime.Now)).TotalDays;
            if (daysGone > 30)
            {
                TempData["Error"] = "Cannot restore — the 30-day window has expired.";
                return RedirectToAction(nameof(Index));
            }

            lesson.IsDeleted = false;
            lesson.DeletedAt = null;
            await _dbContext.SaveChangesAsync();

            TempData["info"] = "Lesson restored successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ────────────────────────────────────────────────────────
        //  BACKGROUND: PURGE EXPIRED LESSONS (no view)
        // ────────────────────────────────────────────────────────
        public async Task DeleteExpiredLessons()
        {
            var expired = await _dbContext.DbLesson
                .Where(l => l.IsDeleted && l.DeletedAt <= DateTime.Now.AddDays(-30))
                .ToListAsync();
            _dbContext.DbLesson.RemoveRange(expired);
            await _dbContext.SaveChangesAsync();
        }
    }
}