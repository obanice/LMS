using Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
	public class RoleViewModel
	{
		public string? UserName { get; set; }
		public string? SelectedRole { get; set; }
		public virtual IdentityRole? Role { get; set; }
		public virtual ApplicationUser? User { get; set; }
		public string? Email { get; set; }
		public string? Id { get; set; }
		public string? Name { get; set; }
		public string? RoleName { get; set; }
		public string? Roles { get; set; }
		public string? StaffId { get; set; }
		public string? RoleId { get; set; }
	}
}
