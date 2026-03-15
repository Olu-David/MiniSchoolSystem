using MiniSchoolSystem.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSchoolSystem.Models
{
    public class LessonEnrollment
    {
        [Key]
        public int Id {  get; set; }
        public int LessonId {  get; set; }
        public int? StudentId { get; set; }
        public string? UserId {  get; set; }
        public bool IsCompleted {  get; set; }
        public Decimal LessonProgress { get; set; }
        public DateTime EnrolledTime { get; set; }

        public DateTime CompletedTime {  get; set; }
        public Sections Sections { get; set; }  

        //Navigation Properties
        [ForeignKey(nameof(UserId))]
        public virtual UserDb? UserDb { get; set; }
        [ForeignKey(nameof(StudentId))]
        public virtual StudentModel? StudentModel { get; set; }
        [ForeignKey(nameof(LessonId))]
        public virtual Lesson? Lesson { get; set; }  
        
    }
}
