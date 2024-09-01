using Core.Constants;
using Core.Db;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace Logic.Helpers
{
	public class AdminHelper : IAdminHelper
	{
		private readonly AppDbContext db;
		private readonly UserManager<ApplicationUser> _userManager;
		public AdminHelper(AppDbContext dbContext, 
			UserManager<ApplicationUser> userManager)
			{
				db = dbContext;
				_userManager = userManager;
			}
		public IPagedList<LecturerViewModel> Lectures(IPageListModel<LecturerViewModel> model, int? departmentId, int page)
		{
			var query = db.ApplicationUsers
					.Where(p => !p.IsDeactivated && p.DepartmentId == departmentId)
					.Include(x=>x.Gender)
					.AsQueryable();
			if (!query.Any())
			{
				return new List<LecturerViewModel>().ToPagedList(page, 25);
			}
			query = query.Where(v => db.UserRoles
					.Any(ur => ur.UserId == v.Id && ur.RoleId == LMSConstants.LecturerRoleId));
			if (!string.IsNullOrEmpty(model.Keyword))
			{
				query = query.Where(v =>
						v.FirstName.ToLower().Contains(model.Keyword.ToLower()) ||
						v.Email.ToLower().Contains(model.Keyword.ToLower()) ||
						v.LastName.ToLower().Contains(model.Keyword.ToLower()) ||
						v.Gender.Name.ToLower().Contains(model.Keyword.ToLower()) ||
						v.PhoneNumber.Contains(model.Keyword.ToLower()));
			}
			if (model.StartDate.HasValue)
			{
				query = query.Where(v => v.DateCreated >= model.StartDate);
			}

			if (model.EndDate.HasValue)
			{
				query = query.Where(v => v.DateCreated <= model.EndDate);
			}
			var lecturers = query
				.OrderByDescending(v => v.DateCreated)
				.Select(v => new LecturerViewModel
				{
					Id = v.Id,
					FullName = v.FullName,
					PhoneNumber = v.PhoneNumber,
					Email = v.Email,
					Gender = v.Gender.Name
				})
				.ToPagedList(page, 25);
			model.Model = lecturers;
			return lecturers;
			
		}
		public async Task<ApplicationUser> AddLecturer(LecturerViewModel lecturerViewModel)
		{
			var applicationUser = new ApplicationUser
			{
				FirstName = lecturerViewModel.FirstName,
				MiddleName = lecturerViewModel.MiddleName,
				LastName = lecturerViewModel.LastName,
				Email = lecturerViewModel.Email,
				PhoneNumber = lecturerViewModel.PhoneNumber,
				GenderId = lecturerViewModel.GenderId,
				DepartmentId = lecturerViewModel.Department,
				UserName = lecturerViewModel.Email
			};
			var result = await _userManager.CreateAsync(applicationUser, "12345").ConfigureAwait(false);
			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(applicationUser, Utility.Constants.LecturerRole).ConfigureAwait(false);
				return applicationUser;
			}
			return new ApplicationUser();
		}
	}
}
