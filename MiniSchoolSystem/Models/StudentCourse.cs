using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSchoolSystem.Models
{
    public class StudentCourse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string? UserDbID { get; set; }

        //Navigation Properties
        public StudentModel? Student { get; set; }   
        public Course? Course { get; set; }
        [ForeignKey(nameof(UserDbID))]
        public UserDb? userDb { get; set; }
    }
}
