using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSchoolSystem.Models
{
    public class LessonContent
    {
        public int TeacherId {  get; set; }

        public int Id {  get; set; }
        public string? FileUrl { get; set; }
        public string? VideoUrl {  get; set; }
        public string? Content {  get; set; }
        public DateTime CreatedAt { get; set; }
        public int? StudentId {  get; set; }


        public int LessonId {  get; set; }
        [ForeignKey(nameof(LessonId))]
        public Lesson? Lesson { get; set; }
        [ForeignKey(nameof(StudentId))]
      public StudentModel? Student { get; set; }
    }
}
