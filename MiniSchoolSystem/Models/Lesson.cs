using MiniSchoolSystem.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSchoolSystem.Models
{
    public class Lesson
    {
        public int Id { get; set; } 
        public int CourseModuleId {  get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? StudentId {  get; set; }
        public string? LessonUserID {  get; set; }
        public Sections LessonSection {  get; set; }
        public ICollection<LessonContent> LessonContent { get; set; }= new List<LessonContent>();   
        public DateTime CreatedAt {  get; set; }  
        public int? TeacherId {  get; set; }
        //Navigation 
        [ForeignKey("StudentId")]
        public StudentModel? Student { get; set; }
        [ForeignKey(nameof(TeacherId))]
        public Teacher? Teacher { get; set; }
        [ForeignKey(nameof(LessonUserID))]
        public UserDb? LessonDb{  get; set; }
        [ForeignKey(nameof(CourseModuleId))]
        public CourseModule? CourseModule { get; set;}
        public bool IsDeleted { get;  set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsArchived { get; internal set; }
        public ICollection<LessonEnrollment> LessonEnrollments { get; set; } = new  List<LessonEnrollment>();   
    }
}
