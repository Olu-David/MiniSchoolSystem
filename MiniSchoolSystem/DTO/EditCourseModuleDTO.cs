using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSchoolSystem.DTO
{
    public class EditCourseModuleDTO
    {
        public int CourseId { get; set; }
        public int TeacherId { get; set; }
        public int Order { get; set; }
        public Sections sections { get; set; }
        public ICollection<EditLessonDTO> Lessons { get; set; }= new List<EditLessonDTO>(); 
        public string? Title { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
    }
}
