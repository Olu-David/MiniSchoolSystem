using MiniSchoolSystem.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSchoolSystem.Models
{
    public class CourseModule
    {
        [Key]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int TeacherId { get; set; }  
        public int Order {  get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;
         public ICollection<Lesson> Lessons { get; set; }= new List<Lesson>();
        public string? Title { get; internal set; }
        //Navigation Properties
        [ForeignKey(nameof(TeacherId))]
        public Teacher? Teacher { get; set; }
        public Sections? CourseSections { get; internal set; }
    }
}
