using MiniSchoolSystem.Enums;

namespace MiniSchoolSystem.DTO
{
    public class CreateCourseModulesViewDTO
    {
     
        public string? Title { get; internal set; }
        public Sections? SelectedSection { get; internal set; }
        public int CourseId { get; set; }
        public int TeacherId { get; set; }
        public int Order { get; set; }
        public ICollection<CreateLessonViewDTO> Lessons { get; set; }=new List<CreateLessonViewDTO>();  
    }
}