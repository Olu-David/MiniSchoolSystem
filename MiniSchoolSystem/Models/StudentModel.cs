using MiniSchoolSystem.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace MiniSchoolSystem.Models
{
    public class StudentModel
    {
        public int Id { get; set; } 
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }
        public Sections StudentSection { get; set; }    
        public string? StudentId { get; set; }
        [ForeignKey(nameof(StudentId))] 
        public UserDb? StudentDb {  get; set; }
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        [ForeignKey(nameof(StudentName))]
        public ICollection< LessonEnrollment>lessonEnrollments  { get; set; }=new List<LessonEnrollment>(); 

        
    }
}
  