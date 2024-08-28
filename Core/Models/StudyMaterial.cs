using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Core.Enum.eLearningEnum;

namespace Core.Models
{
    public class StudyMaterial
    {
        [Key]
        public int? Id { get; set; }
        public string? Description { get; set; }
        public string? FileUrl { get; set; }
        public Levels? Level { get; set; }
        public DateTime DateUploaded { get; set; } = DateTime.Now;
        public string? LecturerId { get; set; }
        [ForeignKey(nameof(LecturerId))]
        public virtual ApplicationUser? Lecturer { get; set; }
        public int? CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public virtual Course? Course { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public virtual Department? Department { get; set; }
    }
}
