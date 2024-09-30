using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
	public class CourseViewModel
	{
		public int? Id { get; set; }
		public string? Code { get; set; }
		public string? Description { get; set; }
		public int? DepartmentId { get; set; }
		public string? Department { get; set; }
		public string? LecturerName { get; set; }
		public string? LecturerId { get; set; }
		public int? SemesterId { get; set; }
		public string? Semester { get; set; }
		public int? LevelId { get; set; }
		public string? Level { get; set; }
		public string? DateCreated { get; set; }
		public string? Name { get; set; }
	}
}
