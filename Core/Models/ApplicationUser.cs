using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Enum.eLearningEnum;

namespace Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public bool IsDeactivated { get; set; }
        public DateTime DateCreated { get; set; }
        public Levels? Level { get; set; }
        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public virtual Department? Department { get; set; }
        public ICollection<Course> Courses { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<StudyMaterial> StudyMaterials { get; set; }
        [NotMapped]
        public string FullName => FirstName + " " + LastName;
        [NotMapped]
        public List<string>? Roles { get; set; }
        [NotMapped]
        public string? UserRole { get; set; }
        [NotMapped]
        public string? RoleId { get; set; }
    }
}
