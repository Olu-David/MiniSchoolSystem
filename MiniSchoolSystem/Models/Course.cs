using MiniSchoolSystem.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSchoolSystem.Models
{
    public class  Course
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CourseUserbID { get; set; }
        public string? Slug { get; set; }
        public DateTime? LastModifiedAt { get;set; }
        public int TeacherID {  get; set; }
        public Sections CourseSections {  get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        //Navigation Properties
        [ForeignKey(nameof(TeacherID))]
        public Teacher? CourseTeacher  { get; set; }    
        public ICollection<CourseModule>CourseModules { get; set; }=new List<CourseModule>();
        public bool IsArchived { get; internal set; }
        [ForeignKey(nameof(CourseUserbID))]
        public UserDb? UserDb { get; set; }
    }
}
