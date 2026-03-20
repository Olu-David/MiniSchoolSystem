using MiniSchoolSystem.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSchoolSystem.Models
{

        public class Teacher
        {
            [Key]
            public int Id { get; set; }

            public string? Name { get; set; }

            // The ONLY direct link to the User Account
            public string TeacherId { get; set; } = string.Empty;

            [ForeignKey(nameof(TeacherId))]
            public virtual UserDb? User { get; set; }

            // The ONLY thing a Teacher "Directly" owns
            public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

            // This is for payroll/admin purposes, not for structure
            public virtual ICollection<TeacherSection> TeacherSections { get; set; } = new List<TeacherSection>();
        }
    }
