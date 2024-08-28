using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Core.Enum.eLearningEnum;

namespace Core.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int? DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department? Department { get; set; }
        public string? LecturerId { get; set; }
        [ForeignKey(nameof(LecturerId))]
        public ApplicationUser? Lecturer { get; set; }
        public Semester? Semester { get; set; }
        public ICollection<Module> Modules { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<StudyMaterial> StudyMaterials { get; set; }
    }
}
