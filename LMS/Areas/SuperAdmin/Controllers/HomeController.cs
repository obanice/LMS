using Core.Db;
using Core.Models;
using Core.ViewModels;
using static Core.Enums.LMSEnum.DropDownEnums;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LMS.Models;
using Newtonsoft.Json;
using Logic.Helpers;
using Microsoft.AspNetCore.Authorization;
using static Logic.AppHttpContext;

namespace LMS.Areas.SuperAdmin.Controllers
{
	[Area("SuperAdmin")]
	[Authorize]
	[SessionTimeout]
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
			if (departmentViewModel == null)
			{
				return ResponseHelper.JsonError("Error occurred");
			}

			// Check if department with the same name already exists
			var isExistingDepartment = _superAdminHelper.CheckExistingDepartmentName(departmentViewModel.Name);
			if (isExistingDepartment)
			{
				return ResponseHelper.JsonError("A department with the same name already exists");
			}

			var departmentAdmin = await _superAdminHelper.AddDepartment(departmentViewModel).ConfigureAwait(false);
			if (departmentAdmin == null)
			{
				return ResponseHelper.JsonError("Unable to create department");
			}

			var userToken = await _emailHelper.CreateUserToken(departmentAdmin.Email).ConfigureAwait(false);
			if (userToken == null)
			{
				return ResponseHelper.JsonError("Admin added successfully, but error occurred while sending mail");
			}

			string linkToClick = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString() + "/Security/Account/ResetPassword?token=" + userToken.Token;
			var sendEmail = _emailHelper.PasswordResetLink(departmentAdmin, linkToClick);

			return ResponseHelper.JsonSuccess("Department added successfully");
		}


		public async Task<IActionResult> Admins()
		{
			var listOfAdmins = await _superAdminHelper.SystemAdmins().ConfigureAwait(false);
			return View(listOfAdmins);
		}

		public JsonResult DeleteDepartment(int? departmentId)
		{
			if (departmentId != null)
			{
				var deleteDpt = _context.Departments.Where(d => d.Id == departmentId && d.Active).FirstOrDefault();
				if (deleteDpt != null)
				{
					deleteDpt.Active = false;
					_context.Departments.Update(deleteDpt);
					_context.SaveChanges();
					return ResponseHelper.JsonSuccess("Deleted Successfully");
				}
			}
			return Json(new { isError = true, msg = "Not found" });

		}
		[HttpPost]
		public JsonResult DeleteAdmin(string? userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				return ResponseHelper.JsonError("Error occurred");
			}
			var user = _context.ApplicationUsers
				.FirstOrDefault(a => a.Id == userId && a.IsDeactivated == false);
			if (user == null)
			{
				return ResponseHelper.JsonError("Error occurred");
			}
			user.IsDeactivated = true;
			_context.Update(user);
			_context.SaveChanges();
			return ResponseHelper.JsonError("Deleted Successfully");

		}
		public IActionResult Lecturer(IPageListModel<ApplicationUserViewModel>? model, int page = 1)
		{
			ViewBag.Layout = UserHelper.GetRoleLayout();
			var lecturer = _superAdminHelper.Lecturer(model, page);
			model.Model = lecturer;
			model.SearchAction = "Lecturer";
			model.SearchController = "Home";
			return View(model);
		}

		public IActionResult Students(IPageListModel<ApplicationUserViewModel>? model, int page = 1)
		{
			ViewBag.Layout = UserHelper.GetRoleLayout();
			var students = _superAdminHelper.GetStudents(model, page);
			model.Model = students;
			model.SearchAction = "Students";
			model.SearchController = "Home";
			return View(model);
		}

	}
}
