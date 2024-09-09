using Core.ViewModels;
using LMS.Controllers;
using LMS.Models;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace LMS.Areas.Lecturer.Controllers
{
	[Area("Lecturer")]
	public class HomeController : BaseController
	{
		private readonly IAdminHelper _adminHelper;
		private readonly ILecturerHelper _lecturerHelper;
		private readonly IDropDownHelper _dropDownHelper;
		private readonly IEmailHelper _emailHelper;
		private readonly IMediaService _mediaService;
		public HomeController(
			IAdminHelper adminHelper,
			IDropDownHelper dropDownHelper,
			IEmailHelper emailHelper,
			IMediaService mediaService,
			ILecturerHelper lecturerHelper)
		{
			_adminHelper = adminHelper;
			_dropDownHelper = dropDownHelper;
			_emailHelper = emailHelper;
			_mediaService = mediaService;
			_lecturerHelper = lecturerHelper;
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

		public IActionResult Material()
		{
			return View();
		}
	}
}
