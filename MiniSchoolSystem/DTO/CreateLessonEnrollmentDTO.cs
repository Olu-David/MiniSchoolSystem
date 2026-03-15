using MiniSchoolSystem.Enums;

namespace MiniSchoolSystem.DTO
{
    public class CreateLessonEnrollmentDTO
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public int? StudentId { get; set; }
        public int UserId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime EnrolledTime { get; set; }
        public DateTime CompletedTime { get; set; }
        public Sections Sections { get; set; }  
    }
}
