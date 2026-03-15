
using MiniSchoolSystem.Enums;

namespace MiniSchoolSystem.DTO
{
    public class EditCourseDTO 
    {
        
    
        public DateTime? LastModifiedAt { get; set; }
        public int Id { get; internal set; }
        public ICollection<EditCourseModuleDTO>EditCourseModules { get; set; } = new List<EditCourseModuleDTO>();
        public string? Title { get; internal set; }
        public Sections SelectedSection {  get; set; }
        public DateTime CreatedAt { get; set; }
    }
}