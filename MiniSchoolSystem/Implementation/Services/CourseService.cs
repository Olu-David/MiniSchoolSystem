using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniSchoolSystem.Controllers;
using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Implementation.Interfaces;
using MiniSchoolSystem.Models;
using System;

namespace MiniSchoolSystem.Implementation.Services
{
   
    public class CourseService : ICourseService
    {
        private readonly UserManager<UserDb> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<LessonController> _logger;
        private readonly IFileService _fileService;

        public CourseService(UserManager<UserDb> userManager, RoleManager<IdentityRole> roleManager, AppDbContext dbContext, ILogger<LessonController> logger, IFileService fileService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _logger = logger;
            _fileService = fileService;
        }
       

        public async Task<(bool Success, string Message, int? CourseId)> CreateCourseAsync(CreateCourseViewDTO model, string userId)
        {
            var User = await _userManager.FindByIdAsync(userId);
            //Get Teacher Id, So that Not just any person can Create course
            var Teacher = await _dbContext.DbTeacher.Include(t => t.TeacherSections).FirstOrDefaultAsync(m => m.TeacherId == userId);
            if (Teacher == null) return (false, "Teacher Not Found", null);

            // 2. Check for duplicate titles for this specific teacher
            bool hasCourse = await _dbContext.DbCourse.AnyAsync(m =>
                m.Title == model.Title && m.TeacherID == Teacher.Id);

            if (hasCourse)
                return (false, "You already have a course with this title.", null);
            var NewCourse = new Course
            {
                Title = model.Title,
                Slug = $"{Guid.NewGuid()}-{model.Title ?? "Null".Replace(" ", "-").ToLower()}",
                CreatedAt = DateTime.UtcNow,
                
               
            };
            try
            {
                _dbContext.DbCourse.Add(NewCourse);
                await _dbContext.SaveChangesAsync();
               
                
                return (true, "Success", NewCourse.Id);
            }
            catch
            {

                return (false, "A database error occurred while saving.", null);
            }


        }

        public async Task<(bool Success, string Message)> DeleteCourseAsync(string userId, int courseId)
        {
            var User = await _userManager.FindByIdAsync(userId);
            var Teacher = await _dbContext.DbTeacher.Include(t => t.TeacherSections).FirstOrDefaultAsync(f => f.TeacherId == userId);
            if (Teacher == null) return (false, "Teacher Not Found");


            // 2. Check if Course is available for deletion
            bool hasCourse = await _dbContext.DbCourse.AnyAsync(m => m.TeacherID == Teacher.Id);

            if (!hasCourse)
            {
                return (false, "Course Doesnt Exist");
            }

            var ExistingCourse = await _dbContext.DbCourse.Include(m => m.CourseModules).ThenInclude(w => w.Lessons).FirstOrDefaultAsync(m =>
                 m.TeacherID == Teacher.Id);
            if (ExistingCourse == null)
            { return (false, "Empty"); }

            ExistingCourse.IsDeleted = true;
            ExistingCourse.DeletedAt = DateTime.UtcNow;

            foreach (var module in ExistingCourse.CourseModules)
                foreach (var lesson in module.Lessons)
                {
                    lesson.IsDeleted = true;
                    lesson.DeletedAt = DateTime.UtcNow;
                }

            await _dbContext.SaveChangesAsync();
            return (true, "Course Deleted, Course available Untill 30days");

        }


        public async Task<(bool Success, string Message)> EditCourseAsync(EditCourseDTO model, string UserId)
        {
            var User = await _userManager.FindByIdAsync(UserId);
            if (User == null) return (false, "User not Found");

            var Teacher = await _dbContext.DbTeacher.Include(w => w.TeacherSections).AsNoTracking().FirstOrDefaultAsync(m => m.TeacherId == UserId);
            if (Teacher == null) return (false, "Teacher doesnt Exist");

            var ExistingCourse = await _dbContext.DbCourse.Include(m => m.CourseModules).AsNoTracking().FirstOrDefaultAsync(m => m.Id == model.Id);
            if (ExistingCourse == null) return (false, "Course No Found");

            ExistingCourse.Title = model.Title;
            ExistingCourse.Slug = $"{Guid.NewGuid()}-{model.Title ?? "Null".Replace(" ", "-").ToLower()}";
            ExistingCourse.LastModifiedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return (true, "Update Successful");


        }
        public async Task<List<Course>> GetAllCourseListAsync()
        {
            var courses = await _dbContext.DbCourse
                           .Where(c => !c.IsDeleted && !c.IsArchived).AsNoTracking()
                           .ToListAsync();
            return courses; ;
        }
            
        public async Task<Course?> GetCourseByIDAsync(int Id)
        {

            var C = await _dbContext.DbCourse.FirstOrDefaultAsync(c => c.Id == Id);
            return C;
        }

        //public async Task<List<Course>> GetCourseBySectionAsync(Sections section)
        //{

        //    return await _dbContext.DbCourse
        // .Where(c => c.CourseSections == section && !c.IsDeleted && !c.IsArchived)
        // .OrderByDescending(c => c.CreatedAt)
        // .ToListAsync();
        //}

        public async Task<List<Course>> GetCourseBySlugAsync(string? slug)
        {
            if (slug == null) throw new ArgumentNullException("slug");
            var Slug = await _dbContext.DbCourse.Where(m => m.Slug == slug).ToListAsync();
            return Slug;
        }

        public async Task<(bool Success, string Message)> RestoreCourseAsync(int CourseId, string Id)
        {
            // 1. Check if User exists (Correction: check for null)
            var User = await _userManager.FindByIdAsync(Id);
            if (User == null) return (false, "User Not Found");

            // 2. Check if Teacher exists
            var Teacher = await _dbContext.DbTeacher
                .Include(i => i.TeacherSections)
                .FirstOrDefaultAsync(m => m.TeacherId == Id);

            if (Teacher == null) return (false, "Teacher Not Found");

            // 3. Get Course with Children
            var course = await _dbContext.DbCourse
                .Include(c => c.CourseModules)
                .ThenInclude(m => m.Lessons)
                .FirstOrDefaultAsync(c => c.Id == CourseId);

            if (course == null) return (false, "Course can't be found");

            // 4. Check if it's actually deleted and within the 30-day window
            if (course.DeletedAt.HasValue)
            {
                var DaysSinceDeleted = (DateTime.UtcNow - course.DeletedAt.Value).TotalDays;

                if (DaysSinceDeleted <= 30)
                {
                    // RESTORE COURSE
                    course.IsDeleted = false;
                    course.DeletedAt = null;

                    foreach (var module in course.CourseModules)
                    {
                        // RESTORE MODULE (Don't skip this!)
                        module.IsDeleted = false;
                        module.DeletedAt = null;

                        foreach (var lesson in module.Lessons)
                        {
                            // RESTORE LESSON
                            lesson.IsDeleted = false;
                            lesson.DeletedAt = null;
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                    return (true, "Course restored successfully!");
                }
                else
                {
                    return (false, "Recovery period (30 days) has expired.");
                }
            }

            return (false, "Course is not currently marked as deleted.");
        }




     public async Task<int> DeleteExpiredCourses()
    {
            var cutoff = DateTime.UtcNow.AddDays(-30);

            var expired = await _dbContext.DbCourse
                .Include(c => c.CourseModules)
                .ThenInclude(m => m.Lessons)
                .Where(c => c.IsDeleted && c.DeletedAt <= cutoff)
                .ToListAsync();

            

            if (!expired.Any()) return 0;

            foreach (var course in expired)
            {
                if (course.CourseModules != null)
                {
                    foreach (var module in course.CourseModules)
                    {
                        if (module.Lessons != null && module.Lessons.Any())
                        {
                            _dbContext.DbLesson.RemoveRange(module.Lessons);
                        }
                    }
     
                    _dbContext.DbModules.RemoveRange(course.CourseModules);
                }
            

                // 2. Remove Modules (The middle layer)
                if (course.CourseModules != null && course.CourseModules.Any())
                {
                    _dbContext.DbModules.RemoveRange(course.CourseModules);
                }
            }

            // 3. Finally, remove the Courses themselves
            _dbContext.DbCourse.RemoveRange(expired);

            await _dbContext.SaveChangesAsync();
            return expired.Count;
    
        }
    }
    
}

    