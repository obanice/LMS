using Core.Models;
using Core.ViewModels;

namespace Logic.IHelpers
{
	public interface ISuperAdminHelper
	{
		Task<ApplicationUser> AddDepartment(AddDepartmentDTO departmentDTO);
		bool CheckExistingDepartmentName(string name);
		List<DepartmentViewModel> GetDepartments();
		SuperAdminViewModel SuperAdminHomeData();
		Task<List<ApplicationUserViewModel>?> SystemAdmins();
	}
}
