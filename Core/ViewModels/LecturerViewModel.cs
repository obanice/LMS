using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
	public class LecturerViewModel
	{
		public string? Id { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		public int? Department { get; set; }
		public string? FullName { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? MiddleName { get; set; }
		public string? Gender { get; set; }
		public int? GenderId { get; set; }
	}
}
