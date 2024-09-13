using Core.ViewModels;
using LMS.Models;
using Logic.Helpers;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using static Core.Enums.LMSEnum;
using static Logic.AppHttpContext;

namespace LMS.Controllers
{
	[Authorize]
	[SessionTimeout]
	public class AdminController : BaseController
	{
		private readonly IAdminHelper _adminHelper;
		private readonly IDropDownHelper _dropDownHelper;
		private readonly IEmailHelper _emailHelper;
		private readonly IMediaService _mediaService;
		public AdminController(
			IAdminHelper adminHelper, 
			IDropDownHelper dropDownHelper,
			IEmailHelper emailHelper,
			IMediaService mediaService)
		{
			_adminHelper = adminHelper;
			_dropDownHelper = dropDownHelper;
			_emailHelper = emailHelper;
			_mediaService = mediaService;
		}
		public IActionResult Index()
		{
			ViewBag.Layout = UserHelper.GetRoleLayout();
			return View();
		}
		[HttpGet]
		public IActionResult Lecturers(IPageListModel<LecturerViewModel>? model, int page = 1)
		{
			ViewBag.Layout = UserHelper.GetRoleLayout();
			ViewBag.Lecturer = _dropDownHelper.GetLecturers();
			ViewBag.Gender = _dropDownHelper.GetDropDownByKey(DropDownEnums.Gender);
			var lecturers = _adminHelper.Lectures(model, CurrentUserDepartmentId, page);
			model.Model = lecturers;
			model.SearchAction = "Lecturers";
			model.SearchController = "Admin";
			return View(model);
		}
		[HttpPost]
		public async Task<JsonResult> AddLecturer(string lecturerDetails)
		{
			try
			{
				if (string.IsNullOrEmpty(lecturerDetails))
				{
					return ResponseHelper.JsonError("Error occurred");
				}
				var lecturerViewModel = JsonConvert.DeserializeObject<LecturerViewModel>(lecturerDetails);
				if (lecturerViewModel == null)
				{
					return ResponseHelper.JsonError("Error occurred");
				}
				lecturerViewModel.Department = CurrentUserDepartmentId;
				if (CurrentUserDepartmentId == 0 || CurrentUserDepartmentId == null)
				{
					return ResponseHelper.JsonError("Error occurred");
				}
				var lecturer = await _adminHelper.AddLecturer(lecturerViewModel).ConfigureAwait(false);
				if (lecturer == null)
				{
					return ResponseHelper.JsonError("Unable to create lecturer");
				}
				var userToken = await _emailHelper.CreateUserToken(lecturer.Email).ConfigureAwait(false);
				if (userToken != null)
				{
					return ResponseHelper.JsonError("Lecturer added successfully, but error occurred while sending mail");
				}
				string linkToClick = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString() + "/Security/Account/ResetPassword?token=" + userToken.Token;
				var sendEmail = _emailHelper.PasswordResetLink(lecturer, linkToClick);
				return ResponseHelper.JsonSuccess("Lecturer added successfully");
			}
			catch (Exception ex)
			{
				LogCritical($"An error occurred when trying to add lecturer with {ex.Message ?? ex.InnerException?.Message}");
				throw;
			}
		}

		[HttpGet]
		public IActionResult Courses(IPageListModel<CourseViewModel>? model, int page = 1)
		{
			ViewBag.Layout = UserHelper.GetRoleLayout();
			ViewBag.Lecturer = _dropDownHelper.GetLecturers();
			ViewBag.Level = _dropDownHelper.GetDropDownByKey(DropDownEnums.Level);
			ViewBag.Semester = _dropDownHelper.GetDropDownByKey(DropDownEnums.Semester);
			var courses = _adminHelper.Courses(model, CurrentUserDepartmentId, string.Empty, page);
			model.Model = courses;
			model.SearchAction = "Courses";
			model.SearchController = "Admin";
			return View(model);
		}

		[HttpPost]
		public JsonResult AddCourse(string courseDetails)
		{
			try
			{
				if (string.IsNullOrEmpty(courseDetails))
				{
					return ResponseHelper.JsonError("Error occurred");
				}
				var courseViewModel = JsonConvert.DeserializeObject<CourseViewModel>(courseDetails);
				if (courseViewModel == null)
				{
					return ResponseHelper.JsonError("Error occurred");
				}
				courseViewModel.DepartmentId = CurrentUserDepartmentId;
				var isCourseAdded = _adminHelper.AddCourse(courseViewModel);
				return isCourseAdded ? ResponseHelper.JsonSuccess("Course added successfully") : ResponseHelper.JsonError("Unable to create course");
			}
			catch (Exception ex)
			{
				LogCritical($"An error occurred when trying to add course with {ex.Message ?? ex.InnerException?.Message}");
				throw;
			}

		}
		public IActionResult LecturerCourse(IPageListModel<CourseViewModel>? model,string lecturerId, int page = 1)
		{
			ViewBag.Layout = UserHelper.GetRoleLayout();
			ViewBag.Lecturer = _dropDownHelper.GetLecturers();
			ViewBag.Gender = _dropDownHelper.GetDropDownByKey(DropDownEnums.Gender);
			var courses = _adminHelper.Courses(model, 0, lecturerId, page);
			model.Model = courses;
			ViewBag.LecturerName = courses?.FirstOrDefault()?.LecturerName;
			model.SearchAction = "LecturerCourse";
			model.SearchController = "Admin";
			return View(model);
		}
		public IActionResult Materials(int? courseId)
		{
			ViewBag.Layout = UserHelper.GetRoleLayout();
			ViewBag.CourseId = courseId;
			var studyMaterials = _adminHelper.GetStudyMaterialsByCoursesById(courseId);
			return View(studyMaterials);
		}
		[HttpPost]
		[DisableRequestSizeLimit]
		public async Task<JsonResult> AddMaterials(int? courseId, IFormFile file)
		{
			if (courseId == 0 || file == null)
			{
				return ResponseHelper.JsonError("Error occurred");
			}
			var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
			var media = await _mediaService.SaveMediaAsync(file.OpenReadStream(), fileName, Utility.Constants.StudyMaterials, _mediaService.GetMediaType(fileName)).ConfigureAwait(false);
			var mediaId = media.Id;
			var isMaterialSaved = _adminHelper.AddMaterial(courseId,mediaId);
			return isMaterialSaved ? ResponseHelper.JsonSuccess($"Saved successfully") : ResponseHelper.JsonError($"Unable to save material");
		}
	}
}
