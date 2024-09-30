using Core.ViewModels;
using LMS.Controllers;
using LMS.Models;
using Logic.Helpers;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using static Core.Enums.LMSEnum;
using static Logic.AppHttpContext;

namespace LMS.Areas.Lecturer.Controllers
{
	[Area("Lecturer")]
	[Authorize]
	[SessionTimeout]
	[Authorize(Roles = "Lecturer")]
	public class HomeController(
		IAdminHelper adminHelper,
		IDropDownHelper dropDownHelper,
		IEmailHelper emailHelper,
		IMediaService mediaService,
		ILecturerHelper lecturerHelper) : BaseController
	{
		private readonly IAdminHelper _adminHelper = adminHelper;
		private readonly ILecturerHelper _lecturerHelper = lecturerHelper;
		private readonly IDropDownHelper _dropDownHelper = dropDownHelper;
		private readonly IEmailHelper _emailHelper = emailHelper;
		private readonly IMediaService _mediaService = mediaService;

		public IActionResult Index()
		{
			
			ViewBag.Layout = UserHelper.GetRoleLayout();
			var lectuereDashboards = _lecturerHelper.GetLecturersDashboardInfo(CurrentUserId);

			return View(lectuereDashboards);
		}
		[HttpPost]
		public async Task<JsonResult> AddStudyMaterial(int courseId, IFormFile file)
		{
			try
			{
				if (courseId > 0)
				{
					return ResponseHelper.JsonError("Error Occurred");
				}
				string fileName = null;
				var studyMaterialViewModel = new StudyMaterialViewModel();
				if (file != null)
				{
					fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
					var media = await _mediaService.SaveMediaAsync(file.OpenReadStream(), fileName, Utility.Constants.StudyMaterials, _mediaService.GetMediaType(fileName)).ConfigureAwait(false);
					studyMaterialViewModel.MediaTypeId = media.Id;
				}
				else
				{
					return ResponseHelper.JsonError("No image selected for this study material ");
				}
				studyMaterialViewModel.CourseId = courseId;
				LogInformation($"Trying to create an study material for course with id - {courseId}");
				var addedMaterial = _lecturerHelper.CreateStudyMaterial(studyMaterialViewModel);
				if (addedMaterial)
				{
					return ResponseHelper.JsonSuccess("study materials Added Successfully");
				}
				LogError($"Unable to add study material for course with id - {courseId}");
				return ResponseHelper.JsonError("Unable to add study material");
			}
			catch (Exception exp)
			{
				LogCritical($"Failed to create study material, this error was thrown${exp.InnerException?.Message ?? exp.Message}");
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
			var courses = _adminHelper.Courses(model, CurrentUserDepartmentId, CurrentUserId, page);
			model.Model = courses;
			model.SearchAction = "Courses";
			model.SearchController = "Home";
			return View(model);
		}
		[HttpGet]
		public IActionResult Quiz(int? courseId, IPageListModel<QuizViewModel>? model, int page = 1)
		{
			ViewBag.Layout = UserHelper.GetRoleLayout();
			ViewBag.Courses = _dropDownHelper.GetCoursesDropDown(CurrentUserId);
			var quizzes = _lecturerHelper.FetchQuizByLecturerId(courseId,model, page,CurrentUserId);
			model.Model = quizzes;
			model.SearchAction = "Quiz";
			model.SearchController = "Home";
			return View(model);
		}
		[HttpPost]
		[DisableRequestSizeLimit]
		public async Task<JsonResult> AddQuiz(int? courseId, IFormFile file)
		{
			if (courseId == 0 || file == null)
			{
				return ResponseHelper.JsonError("Error occurred");
			}
			var quizDTO = new QuizDTO();
			var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
			var media = await _mediaService.SaveMediaAsync(file.OpenReadStream(), fileName, Utility.Constants.Quiz, _mediaService.GetMediaType(fileName)).ConfigureAwait(false);
			quizDTO.QuestionId = media.Id;
			quizDTO.CourseId = courseId;
			quizDTO.LecturerId = CurrentUserId;
			var isQuizSet = _lecturerHelper.AddQuiz(quizDTO);
			return isQuizSet ? ResponseHelper.JsonSuccess($"Quiz set successfully") : ResponseHelper.JsonError($"Unable to set quiz");
		}
		[HttpGet]
		public IActionResult QuizAnswers(int? quizId, IPageListModel<QuizAnswersViewModel>? model, int page = 1)
		{
			ViewBag.Layout = UserHelper.GetRoleLayout();
			var quizAnswers = _lecturerHelper.FetchQuizAnswersByQuizId(quizId, model, page);
			model.Model = quizAnswers;
			model.SearchAction = "QuizAnswers";
			model.SearchController = "Home";
			return View(model);
		}
		[HttpPost]
		public JsonResult AddScore(int quizId, decimal? mark)
		{
			var quizAnswer = _lecturerHelper.AddScoreToQuiz(quizId, mark);
			return quizAnswer == null ? ResponseHelper.JsonError("Error Occurred") : ResponseHelper.JsonSuccess("Score Added successfully");
		}
	}
}
