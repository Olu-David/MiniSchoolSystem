using MiniSchoolSystem.Enums;
using System.ComponentModel.DataAnnotations;

namespace MiniSchoolSystem.DTO
{
    public class CreateCourseViewDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Enter Title")]
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public Sections SelectedSection {  get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public ICollection<CreateCourseModulesViewDTO> CreateCourseModules { get; set; }=new List<CreateCourseModulesViewDTO>(); 
    }
}
