using Microsoft.AspNetCore.Identity;
using MiniSchoolSystem.Enums;

namespace MiniSchoolSystem.Models
{
    public class UserDb : IdentityUser
    {
        public string? FullName { get; set; }
        public int? StudentId { get; set; }
        public int? LessonId { get; set; }
        public Sections? UserSection { get; set; }
       



        //Navigation Properties 
        public virtual StudentModel? Student { get; set; }
     
        public virtual CourseModule? CourseModule { get; set; }
        public ICollection<LessonEnrollment>?LessonEnrollments { get; set; } = new List<LessonEnrollment>();
        public ICollection<Lesson>Lessons { get; set; }= new List<Lesson>();    
    }
}
