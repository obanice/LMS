using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Enums.LMSEnum;

namespace Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            IsDeactivated = false;
            DateCreated = DateTime.Now;
            IsDepartmentAdmin = false;
        }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public bool IsDeactivated { get; set; }
        public DateTime DateCreated { get; set; }
		public bool IsDepartmentAdmin { get; set;}
		public int? GenderId { get; set; }
		[ForeignKey(nameof(GenderId))]
		public virtual CommonDropDown? Gender { get; set; }
        public int? LevelId { get; set; }
        [ForeignKey(nameof(LevelId))]
        public virtual CommonDropDown? Level { get; set; }
        public int? SemesterId { get; set; }
        [ForeignKey(nameof(SemesterId))]
        public virtual CommonDropDown? Semester { get; set; }
        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public virtual Department? Department { get; set; }
        public ICollection<Course> Courses { get; set; }
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
