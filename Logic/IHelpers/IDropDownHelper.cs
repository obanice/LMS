using Core.Models;
using Core.ViewModels;
using static Core.Enums.LMSEnum;
using static Logic.Helpers.DropDownHelper;

namespace Logic.IHelpers
{
	public interface IDropDownHelper
	{
		Task<bool> CreateDropDownsAsync(CommonDropDown commonDropDowns);
		bool DeleteDropDownById(int id);
		bool EditDropDown(CommonDropDown commonDropdowns);
		List<DepartmentViewModel> GetDepartments();
		List<CommonDropDown> GetDropDownByKey(DropDownEnums dropDownKey);
		List<DropdownEnumModel> GetDropDownEnumList();
		DropdownEnumModel GetEnumById(int enumkeyId);
		List<ApplicationUserViewModel> GetLecturers();
		List<ApplicationUserViewModel> GetUsers();
	}
}
