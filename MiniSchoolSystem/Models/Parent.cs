using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSchoolSystem.Models
{
    public class Parent
    {
        public int Id { get; set; }
        public string? ParentId { get; set; }  
        [ForeignKey(nameof(ParentId))] 
        public UserDb ?UserDb { get; set; }
    }
}
