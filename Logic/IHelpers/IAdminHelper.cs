using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Logic.IHelpers
{
	public interface IAdminHelper
	{
		bool AddCourse(CourseViewModel courseViewModel);
		Task<ApplicationUser> AddLecturer(LecturerViewModel lecturerViewModel);
		IPagedList<CourseViewModel> Courses(IPageListModel<CourseViewModel> model, int? departmentId, int page);
		IPagedList<LecturerViewModel> Lectures(IPageListModel<LecturerViewModel> model, int? departmentId, int page);
	}
}
