using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
	public class Course:BaseModel
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }
        public string? LecturerId { get; set; }
        [ForeignKey(nameof(LecturerId))]
        public ApplicationUser? Lecturer { get; set; }
        public int? SemesterId { get; set; }
        [ForeignKey(nameof(SemesterId))]
        public virtual CommonDropDown? Semester { get; set; }
        public int? LevelId { get; set; }
        [ForeignKey(nameof(LevelId))]
        public virtual CommonDropDown? Level { get; set; }
        public ICollection<Quiz> Quizzes { get; set; }
        public ICollection<StudyMaterial> StudyMaterials { get; set; }
    }
}
