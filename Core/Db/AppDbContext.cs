using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Core.Db
{
	public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<LecturerCourses> LecturerDepartments { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<ScreenRole> ScreenRoles { get; set; }
        public DbSet<StudyMaterial> StudyMaterials { get; set; }
        public DbSet<CommonDropDown> CommonDropDowns { get; set; }
        public DbSet<UserVerification> UserVerifications { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Quiz> Quiz { get; set; }
    }
}
