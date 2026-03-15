using MiniSchoolSystem.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSchoolSystem.Models
{
    public class TeacherSection
    {
        public int Id {  get; set; }
        public Sections? TSection {  get; set; }
        public int TeacherId { get; set; }
        //Navigation Properties
        [ForeignKey(nameof(TeacherId))]
        public Teacher? Teacher { get; set; }

    }
}
