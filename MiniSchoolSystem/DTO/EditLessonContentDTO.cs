namespace MiniSchoolSystem.DTO
{
    public class EditLessonContentDTO
    {
        public int Id { get; set; }
        public string? FileUrl { get; set; }
        public IFormFile? file { get; set; }
        public string? VideoUrl { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt
        {
            get; set;
        }
    }
}
