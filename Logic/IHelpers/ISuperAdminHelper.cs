using Core.Models;
using Core.ViewModels;

namespace Logic.IHelpers
{
	public interface ISuperAdminHelper
	{
		Task<ApplicationUser> AddDepartment(AddDepartmentDTO departmentDTO);
		List<DepartmentViewModel> GetDepartments();
		SuperAdminViewModel SuperAdminHomeData();
	}
}
