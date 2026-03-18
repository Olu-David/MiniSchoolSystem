using MiniSchoolSystem.DTO;
using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Models;

namespace MiniSchoolSystem.Implementation.Interfaces
{
    public interface ICourseService
    {
        
            // Create: Returns a Tuple with the new ID for redirection
        Task<(bool Success, string Message, int? CourseId)> CreateCourseAsync(CreateCourseViewDTO model, string userId);

            // Read (List): Get all courses (usually for an Admin)
        Task<List<Course>> GetAllCourseListAsync();

            // Read (Filter): Get courses based on User's section
        Task<List<Course>> GetCourseBySectionAsync(Sections section);

            // Read (Single): Get by ID or Slug
        Task<Course?> GetCourseByIDAsync(int courseId);
        Task<List<Course>> GetCourseBySlugAsync(string? slug);

        // Update: Returns a string (the error message, or null/empty if success)
        Task<(bool Success, string Message)> EditCourseAsync(EditCourseDTO model, string UserId);

        //Soft Delete

        Task<(bool Success, string Message)> RestoreCourseAsync(int CourseId, string Id);
             // Delete: Better to return a bool to indicate if deletion happened

        Task<int> DeleteExpiredCourses();
         Task<(bool Success, string Message)> DeleteCourseAsync(string userId, int courseId);
        }
    }

