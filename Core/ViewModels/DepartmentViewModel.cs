using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Enum.LMSEnum;

namespace Core.ViewModels
{
	public class DepartmentViewModel
	{
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? AdminName { get; set; }
        public string? AdminPhoneNumber { get; set; }
    }
	public class AddDepartmentDTO
	{
		public string? Name { get; set; }
		public string? Email { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? MiddleName { get; set; }
		public string? PhoneNumber { get; set; }
		public Levels? Level { get; set; }
		public int? GenderId { get; set; }
		public string? UserId { get; set; }
		public int? DepartmentId { get; set; }
	}
}
