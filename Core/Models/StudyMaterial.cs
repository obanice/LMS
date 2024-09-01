using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Core.Enum.LMSEnum;

namespace Core.Models
{
    public class StudyMaterial
    {
        [Key]
        public int Id { get; set; }
        public bool Active { get; set; }
        public string? FileUrl { get; set; }
        public DateTime DateUploaded { get; set; } = DateTime.Now;
        public int? CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public virtual Course? Course { get; set; }
    }
}
