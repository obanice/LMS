using Core.ViewModels;
using LMS.Controllers;
using LMS.Models;
using Logic.Helpers;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using static Logic.AppHttpContext;

namespace LMS.Areas.Student.Controllers
{
	[Area("Student")]
	[Authorize]
	[SessionTimeout]
	[Authorize(Roles = "Student")]
	public class HomeController(
		IAdminHelper adminHelper,
		IDropDownHelper dropDownHelper,
		IEmailHelper emailHelper,
		IMediaService mediaService,
		IStudentHelper studentHelper) : BaseController
	{
		private readonly IAdminHelper _adminHelper = adminHelper;
		private readonly IStudentHelper _studentHelper = studentHelper;
		private readonly IDropDownHelper _dropDownHelper = dropDownHelper;
		private readonly IEmailHelper _emailHelper = emailHelper;
		private readonly IMediaService _mediaService = mediaService;

		public IActionResult Index()
		{
			ViewBag.Layout = UserHelper.GetRoleLayout();
			var courseMatrials = _studentHelper.GetStudyMaterials(CurrentUserDepartmentId);
			return View(courseMatrials);
		}
		public IActionResult Courses(IPageListModel<CourseViewModel>? model, int page = 1)
		{
			ViewBag.Layout = UserHelper.GetRoleLayout();
			var user = Utility.GetCurrentUser();
			var courses = _studentHelper.Courses(model, user.DepartmentId, user.LevelId, page);
			model.Model = courses;
			model.SearchAction = "Courses";
			model.SearchController = "Home";
			return View(model);
		}
		public IActionResult Materials()
		{
			ViewBag.Layout = UserHelper.GetRoleLayout();
			var studyMaterials = _studentHelper.GetStudyMaterials(CurrentUserDepartmentId);
			return View(studyMaterials);
		}
		[HttpGet]
		public IActionResult Quiz(IPageListModel<QuizViewModel>? model,int? courseId, int page = 1)
		{
			ViewBag.Layout = UserHelper.GetRoleLayout();
			var quizzes = _studentHelper.FetchAllQuiz(model, page, courseId);
			model.Model = quizzes;
			model.SearchAction = "Quiz";
			model.SearchController = "Home";
			return View(model);
		}
		[HttpPost]
		[DisableRequestSizeLimit]
		public async Task<JsonResult> UploadAnswer(int? quizId, IFormFile file)
		{
			if (quizId == 0 || file == null)
			{
				return ResponseHelper.JsonError("Error occurred");
			}
			var quizDTO = new QuizAnswersDTO();
			var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"');
			var media = await _mediaService.SaveMediaAsync(file.OpenReadStream(), fileName, Utility.Constants.Quiz, _mediaService.GetMediaType(fileName)).ConfigureAwait(false);
			quizDTO.AnswerId = media.Id;
			quizDTO.QuizId = quizId;
			quizDTO.StudentId = CurrentUserId;
			var isAnsUploaded = _studentHelper.UploadAnswer(quizDTO);
			return isAnsUploaded ? ResponseHelper.JsonSuccess($"Answer uploaded successfully") : ResponseHelper.JsonError($"Unable to upload answer");
		}
		[HttpGet]
		public IActionResult Scores(IPageListModel<QuizAnswersViewModel>? model, int page = 1)
		{
			ViewBag.Layout = UserHelper.GetRoleLayout();
			var quizAnswers = _studentHelper.FetchQuizAnswersByStudentId(CurrentUserId, model, page);
			model.Model = quizAnswers;
			model.SearchAction = "Scores";
			model.SearchController = "Home";
			return View(model);
		}
	}
}
