using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MiniSchoolSystem.DTO
{
    public class CreateLessonViewDTO
    {
        public int Id { get; set; }
        public int CourseModuleId { get; set; }
        [Required(ErrorMessage ="Enter LessonTitle"), StringLength(100, MinimumLength =8)]
        [DisplayName]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Enter Description"), StringLength(100, MinimumLength = 8)]
        [DisplayName("Description")]
        public string? Description { get; set; }
        public  IFormFile? AttachmentFile { get; set; }
        public int StudentId { get; set; }
        public int LessonUserID { get; set; }
        public Sections LessonSection { get; set; }
        public ICollection<CreateLessonContentDTO> LessonContent { get; set; } = new List<CreateLessonContentDTO>();
        public int TeacherId { get; set; }

    }
}
