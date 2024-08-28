using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    public class Module
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Content { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int? CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public virtual Course? Course { get; set; }
        public ICollection<Lesson> Lessons { get; set; }
    }
}
