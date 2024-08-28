
using static Core.Enum.eLearningEnum;

namespace Core.ViewModels
{
	public class ApplicationUserViewModel
	{
		public string? Id { get; set; }
		public string? Email { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Password { get; set; }
		public string? Role { get; set; }
		public string? ConfirmPassword { get; set; }
		public string? PhoneNumber { get; set; }
		public int? DepartmentId { get; set; }
		public Levels? Level { get; set; }
        public string FullName => FirstName + " " + LastName;
	}
}
