using Core.Db;
using Core.Models;
using Core.ViewModels;
using static Core.Enum.LMSEnum.DropDownEnums;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LMS.Models;
using Newtonsoft.Json;

namespace LMS.Areas.SuperAdmin.Controllers
{
	[Area("SuperAdmin")]
	public class HomeController : Controller
	{
		private readonly AppDbContext _context;
		private readonly IDropDownHelper _dropdownHelper;
		private readonly IUserHelper _userHelper;
		private readonly ISuperAdminHelper _superAdminHelper;
		private readonly IAdminHelper _adminHelper;
		private readonly IEmailHelper _emailHelper;
		private readonly UserManager<ApplicationUser> _userManager;
		public HomeController(
			AppDbContext context,
			IDropDownHelper dropdownHelper,
		   IUserHelper userHelper,
		   ISuperAdminHelper superAdminHelper,
		   UserManager<ApplicationUser> userManager,
		   IAdminHelper adminHelper,
		   IEmailHelper emailHelper
			)
		{
			_context = context;
			_dropdownHelper = dropdownHelper;
			_userHelper = userHelper;
			_superAdminHelper = superAdminHelper;
			_userManager = userManager;
			_adminHelper = adminHelper;
			_emailHelper = emailHelper;
		}
		[HttpGet]
		public IActionResult Index()
		{
			ViewBag.Gender = _dropdownHelper.GetDropDownByKey(Gender);
			var homeData = _superAdminHelper.GetDepartments();
			return View(homeData);
		}
		[HttpPost]
		public async Task<JsonResult> Add(string departmentDetails)
		{
			if (string.IsNullOrEmpty(departmentDetails))
			{
				return ResponseHelper.JsonError("Error occurred");
			}
			var departmentViewModel = JsonConvert.DeserializeObject<AddDepartmentDTO>(departmentDetails);
			if (departmentViewModel == null) {
				return ResponseHelper.JsonError("Error occurred");
			}
			var departmentAdmin =await _superAdminHelper.AddDepartment(departmentViewModel).ConfigureAwait(false);
			if (departmentAdmin == null)
			{
				return ResponseHelper.JsonError("Unable to create department");
			}
			var userToken = await _emailHelper.CreateUserToken(departmentAdmin.Email).ConfigureAwait(false);
			if (userToken != null)
			{
				return ResponseHelper.JsonError("School added successfully, but error occurred while adding school admin");
			}
			string linkToClick = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString() + "/Security/Account/ResetPassword?token=" + userToken.Token;
			var sendEmail = _emailHelper.PasswordResetLink(departmentAdmin, linkToClick);
			return ResponseHelper.JsonSuccess("Department added successfully");
		}
	}
}
