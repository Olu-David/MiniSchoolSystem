using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniSchoolSystem.Controllers
{
    [Authorize(Roles ="Student")]
    public class StudentController : Controller
    {
        private readonly UserManager<UserDb> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private ILogger<StudentController> _logger;
        private readonly AppDbContext _DbContext;

        public StudentController(UserManager<UserDb> userManager, RoleManager<IdentityRole> roleManager, ILogger<StudentController> logger, AppDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _DbContext = dbContext;
        }
        [HttpGet]
        public IActionResult DisplayUserName()
        {
            var UserId = _userManager.GetUserId(User);
            if(UserId == null)
            {
                return Unauthorized();
            }
           string userName = User.Identity?.Name??"Guest";
            return View("Welcome", userName);
        }
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 15)
        { 
            var UserId=User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Student = await _DbContext.DbStudents.FirstOrDefaultAsync(m => m.StudentId == UserId);
            if(Student==null)
            {
                return BadRequest("Student Does not exist");

            }
         
            

            // 1. Calculate how many records to skip
            // If we are on page 2 and size is 5, we skip (2-1)*5 = 5 records.
            int excludedRecords = (pageNumber - 1) * pageSize;
            // 2. The Paginated Query
            var viewCourse = await _DbContext.DbCourse
                .Include(m => m.CourseModules)
                    .ThenInclude(l => l.Lessons)
                        .ThenInclude(c => c.LessonContents)
                .Where(m => m.Id == Student.Id && Student.StudentSection == m.CourseSections&& !m.IsArchived&&!m.IsDeleted)
                .OrderBy(m => m.CreatedAt) // 3. IMPORTANT: Always order before skipping!
                .Skip(excludedRecords)     // Jump over previous pages
                .Take(pageSize)            // Grab only the amount for this page
                .ToListAsync();

            var CompletedLesson= await _DbContext.DbLessonEnrollments.Where(e=>e.IsCompleted).ToListAsync();
            var CountCompletedLesson= await _DbContext.DbLessonEnrollments.CountAsync(m=>m.IsCompleted);

            var TotalNumbersOfStudent= await _DbContext.DbStudents.CountAsync();
            var TotalNumberofCourse = await _DbContext.DbCourse.CountAsync(c => !c.IsArchived && !c.IsDeleted);


            
            return View();




        }
        [HttpGet]
        public async Task<IActionResult> CreateLessonEnrollment()
        {
            //Verification
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Student = await _DbContext.DbStudents.FirstOrDefaultAsync(s => s.StudentId == UserId);
            if (Student == null)
            {
                return Unauthorized();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> CreateLessonEnrollment(CreateLessonEnrollmentDTO model)
        {
            // 1. Get current Student/User ID as a string (Matches Identity UserDb)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Check If Student Is enrolled
            
            var alreadyEnrolled = await _DbContext.DbLessonEnrollments
                .AnyAsync(x => x.StudentId == userId && x.LessonId == model.LessonId);

            if (alreadyEnrolled)
            {
                // If they are already enrolled, don't create a duplicate. 
                // Just send them back to the lesson list or the specific lesson.
                TempData["Info"] = "You are already enrolled in this lesson.";
                return RedirectToAction("Details", "Lesson", new { id = model.LessonId });
            }

            // 3. Create the New Enrollment Object
            var newEnrollment = new LessonEnrollment
            {
                StudentId = userId,           // String ID from Identity
                LessonId = model.LessonId, // Int ID from your DTO
                Sections = model.Sections,
                IsCompleted = false,       // New enrollments aren't finished yet
                EnrolledTime = DateTime.UtcNow,
                LessonProgress = 0
            };

            // 4. SAVE TO DATABASE: Your previous code was missing these two lines!
            _DbContext.DbLessonEnrollments.Add(newEnrollment);
            await _DbContext.SaveChangesAsync();

            TempData["Success"] = "Lesson started! Good luck with your studies.";

            // Redirect to the actual lesson view so they can start learning
            return RedirectToAction("Watch", "Lesson", new { id = model.LessonId });
        }
        

       
        static string StudentRegistrationCode(Sections sections)
        {
            var getCode = Guid.NewGuid().ToString().Substring(0,6);
            var BeginAt = DateTime.Now.ToString("yyyMMdd");

            string prefix = sections switch
            {
                Sections.Kg => "KG",
                Sections.MiddleSchool => "JSS",
                Sections.HighSchool => "SS",
                _ => ""
            };


            return prefix + "/" + "/" + getCode + BeginAt.ToString();
        }
        
    }
}
