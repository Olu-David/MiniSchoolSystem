using MiniSchoolSystem.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSchoolSystem.Models
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty; // Added default to avoid null errors

        public string? Description { get; set; }

        public Sections LessonSection { get; set; }
        public int TeacherId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key to the Parent (Module)
        public int CourseModuleId { get; set; }

        [ForeignKey(nameof(CourseModuleId))]
        public virtual CourseModule? CourseModule { get; set; }

        // Navigation Properties
        public virtual ICollection<LessonContent> LessonContents { get; set; } = new List<LessonContent>();

        // This is where the Student progress lives
        public virtual ICollection<LessonEnrollment> LessonEnrollments { get; set; } = new List<LessonEnrollment>();

        // Soft Delete & Archive Status
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsArchived { get; set; } // Changed to 'set' for easier updates
    }
}