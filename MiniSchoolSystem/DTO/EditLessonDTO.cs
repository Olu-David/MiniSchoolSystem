using MiniSchoolSystem.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MiniSchoolSystem.DTO
{
    public class EditLessonDTO
    {
        public int Id { get; set; }
        public int CourseModuleId { get; set; }
        [Required(ErrorMessage = "Enter LessonTitle"), StringLength(100, MinimumLength = 8)]
        [DisplayName]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Enter Description"), StringLength(100, MinimumLength = 8)]
        [DisplayName("Description")]
        public string? Description { get; set; }
        public int StudentId { get; set; }
        public int LessonUserID { get; set; }
        public Sections LessonSection { get; set; }
        public ICollection<EditLessonContentDTO>editLessonContentDTOs { get; set; }= new List<EditLessonContentDTO>();  
    }
}