using Core.Db;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Logic.Helpers
{
	public class LecturerHelper : BaseHelper, ILecturerHelper
	{
		private readonly UserManager<ApplicationUser> _userManager;
		public LecturerHelper(AppDbContext dbContext,
			UserManager<ApplicationUser> userManager) : base(dbContext)
		{
			_userManager = userManager;
		}

		public bool CreateStudyMaterial(StudyMaterialViewModel studyMaterial)
		{
			return Create<StudyMaterialViewModel, StudyMaterial>(studyMaterial);
		}
	}
}
