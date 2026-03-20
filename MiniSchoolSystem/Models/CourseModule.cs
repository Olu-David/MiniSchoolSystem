using MiniSchoolSystem.Enums;
using MiniSchoolSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CourseModule
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CourseId { get; set; }

    // CHANGE: Made this nullable (int?) so Admins can create 
    // modules before assigning a specific teacher.
    public int? TeacherId { get; set; }

    public int Order { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    // Navigation Properties
    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey(nameof(TeacherId))]
    public virtual Teacher? Teacher { get; set; }

   
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public Sections? CourseSections { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}