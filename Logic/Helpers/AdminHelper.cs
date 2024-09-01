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
		public IPagedList<CourseViewModel> Courses(IPageListModel<CourseViewModel> model, int? departmentId, int page)
		{
			var query = db.Courses
					.Where(p => p.Active && p.DepartmentId == departmentId)
					.Include(x => x.Lecturer)
					.Include(x => x.Department)
					.Include(x => x.Semester)
					.Include(x => x.Level)
					.AsQueryable();
			if (!query.Any())
			{
				return new List<CourseViewModel>().ToPagedList(page, 25);
			}
			if (!string.IsNullOrEmpty(model.Keyword))
			{
				query = query.Where(v =>
						v.Code.ToLower().Contains(model.Keyword.ToLower()) ||
						v.Name.ToLower().Contains(model.Keyword.ToLower()) ||
						v.Description.ToLower().Contains(model.Keyword.ToLower()) ||
						v.Lecturer.FirstName.ToLower().Contains(model.Keyword.ToLower()) ||
						v.Department.Name.Contains(model.Keyword.ToLower()));
			}
			if (model.StartDate.HasValue)
			{
				query = query.Where(v => v.DateCreated >= model.StartDate);
			}

			if (model.EndDate.HasValue)
			{
				query = query.Where(v => v.DateCreated <= model.EndDate);
			}
			var courses = query
				.OrderByDescending(v => v.DateCreated)
				.Select(v => new CourseViewModel
				{
					Id = v.Id,
					Name = v.Name,
					Code = v.Code,
					Description = v.Description,
					LecturerName = v.Lecturer.FullName,
					Department = v.Department.Name
				})
				.ToPagedList(page, 25);
			model.Model = courses;
			return courses;

		}

		public bool AddCourse(CourseViewModel courseViewModel)
		{
			var course = new Course
			{
				Name = courseViewModel.Name,
				Code = courseViewModel.Code,
				Description = courseViewModel.Description,
				LecturerId = courseViewModel.LecturerId,
				SemesterId = courseViewModel.SemesterId,
				DepartmentId = courseViewModel.DepartmentId,
			};
			db.Add(course);
			db.SaveChanges();
			return true;
		}
	}
}
