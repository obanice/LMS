
using static Core.Enums.LMSEnum;

namespace Core.ViewModels
{
	public class ApplicationUserViewModel
	{
		public string? Id { get; set; }
		public string? Email { get; set; }
		public string? FirstName { get; set; }
		public string? DropDownName { get; set; }
		public string? LastName { get; set; }
		public string? Password { get; set; }
		public string? Role { get; set; }
		public string? ConfirmPassword { get; set; }
		public string? PhoneNumber { get; set; }
		public int? DepartmentId { get; set; }
		public int? LevelId { get; set; }
        public string? FullName { get; set; }
	}
}
