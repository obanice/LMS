using Core.Models;
using Core.ViewModels;
using X.PagedList;

namespace Logic.IHelpers
{
	public interface ISuperAdminHelper
	{
		Task<ApplicationUser> AddDepartment(AddDepartmentDTO departmentDTO);
		bool CheckExistingDepartmentName(string name);
		List<DepartmentViewModel> GetDepartments();
		SuperAdminViewModel SuperAdminHomeData();
		Task<List<ApplicationUserViewModel>?> SystemAdmins();
		IPagedList<ApplicationUserViewModel> Lecturer(IPageListModel<ApplicationUserViewModel> model, int page);
		IPagedList<ApplicationUserViewModel> GetStudents(IPageListModel<ApplicationUserViewModel> model, int page);
	}
}
