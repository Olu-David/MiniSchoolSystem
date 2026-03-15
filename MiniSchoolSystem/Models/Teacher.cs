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
        public string? TeacherId {  get; set; }
        //Navigation Properties
        [ForeignKey(nameof(TeacherId))]
        public UserDb?TeacherDb { get; set; }
        public ICollection<Lesson>? Lessons { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>(); 
        public ICollection<StudentModel>? StudentTeacher { get; set; }    
        public ICollection<TeacherSection>TeacherSections { get; set; }= new List<TeacherSection>();    
    }
}
