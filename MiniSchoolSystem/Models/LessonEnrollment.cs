using MiniSchoolSystem.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSchoolSystem.Models
{
    public class LessonEnrollment
    {
      
            [Key]
            public int Id { get; set; }

            public int LessonId { get; set; }

            // Use ONLY the string ID from Identity for the link
            public string? StudentId { get; set; }

            public bool IsCompleted { get; set; }
            public decimal LessonProgress { get; set; } // Use 'decimal' (lowercase)
            public DateTime EnrolledTime { get; set; } = DateTime.UtcNow;
            public DateTime? CompletedTime { get; set; } // Should be nullable if not finished!

            public Sections Sections { get; set; }

            // Navigation Properties
            [ForeignKey(nameof(StudentId))]
            public virtual UserDb? UserDb { get; set; }

            [ForeignKey(nameof(LessonId))]
            public virtual Lesson? Lesson { get; set; }
        }

    }

