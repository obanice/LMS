using Core.Constants;
using Core.Db;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;
using X.PagedList;

namespace Logic.Helpers
{
	public class SuperAdminHelper:ISuperAdminHelper
	{
		private readonly AppDbContext db;
		private readonly UserManager<ApplicationUser> _userManager;
		public SuperAdminHelper(
			AppDbContext dbContext, 
			UserManager<ApplicationUser> userManager
			)
		{
			db = dbContext;
			_userManager = userManager;
		}
		public SuperAdminViewModel SuperAdminHomeData()
		{
			var superAdminViewModel = new SuperAdminViewModel();
			superAdminViewModel.DepartmentViewModels = GetDepartments();
			return superAdminViewModel;
		}
		public List<DepartmentViewModel> GetDepartments()
		{
			
			var schoolViewModel = new List<DepartmentViewModel>();
			var departments = db.Departments
				.Where(x =>  x.Active)
				.ToList();
			foreach (var department in departments)
			{
				var departmentAdmin = GetDepartmentAdminByDepartmentId(department.Id);
				var schoolDetails = new DepartmentViewModel()
				{
					Id = department.Id,
					Name = department.Name,
					AdminName = departmentAdmin?.FullName,
					AdminPhoneNumber = departmentAdmin?.PhoneNumber,

				};
				schoolViewModel.Add(schoolDetails);
			}
			return schoolViewModel;
		}
		public ApplicationUser? GetDepartmentAdminByDepartmentId(int departmentId)
		{
			return db.ApplicationUsers
					.FirstOrDefault(x => x.DepartmentId == departmentId && !x.IsDeactivated && x.IsDepartmentAdmin); ;
		}

		public bool CheckExistingDepartmentName(string name)
		{
			if (name != null)
			{
				var checkName = db.Departments.Where(x => x.Name.ToLower() == name && x.Active).FirstOrDefault();
				if (checkName != null)
				{
					return true;
				}
			}
			return false;
		}
		public async Task<ApplicationUser> AddDepartment(AddDepartmentDTO departmentDTO)
		{
			var department = new Department
			{
				Name = departmentDTO.Name
			};
			db.Add(department);
			db.SaveChanges();
			var user = await AddDepartmentAdminAndAddToRole(departmentDTO, department.Id).ConfigureAwait(false);
			return user;
		}
		public async Task<ApplicationUser> AddDepartmentAdminAndAddToRole(AddDepartmentDTO departmentDTO, int departmentId)
		{
			var applicationUser = new ApplicationUser()
			{
				UserName = departmentDTO.Email,
				FirstName = departmentDTO.FirstName,
				LastName = departmentDTO.LastName,
				MiddleName = departmentDTO.MiddleName,
				Email = departmentDTO.Email,
				PhoneNumber = departmentDTO.PhoneNumber,
				GenderId = departmentDTO.GenderId,
				DepartmentId = departmentId,
				IsDepartmentAdmin = true
			};
			var result = await _userManager.CreateAsync(applicationUser, "12345").ConfigureAwait(false);
			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(applicationUser, Utility.Constants.AdminRole).ConfigureAwait(false);
				return applicationUser;
			}
			return new ApplicationUser();
		}

		public async Task<List<ApplicationUserViewModel>?> SystemAdmins()
		{
			var users = new List<ApplicationUserViewModel>();
			var systemAdmin = await _userManager
				.GetUsersInRoleAsync(Utility.Constants.AdminRole)
				.ConfigureAwait(false);
			return systemAdmin?.Select(x => new ApplicationUserViewModel
			{
				Id = x.Id,
				Email = x.Email,
				PhoneNumber = x.PhoneNumber,
				FullName = x.FullName,
			}).ToList();
		}
		public IPagedList<ApplicationUserViewModel> Lecturer(IPageListModel<ApplicationUserViewModel> model, int page)
		{
			var query = db.ApplicationUsers.Where(p => !p.IsDeactivated).Include(x => x.Gender).AsQueryable();

			if (!query.Any())
			{
				return new List<ApplicationUserViewModel>().ToPagedList(page, 25);
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
			var Lecturer = query
				.OrderByDescending(v => v.DateCreated)
				.Select(v => new ApplicationUserViewModel
				{
					Id = v.Id,
					FullName = v.FullName,
					PhoneNumber = v.PhoneNumber,
					Email = v.Email,
					DropDownName = v.Gender.Name
				})
				.ToPagedList(page, 25);
			model.Model = Lecturer;
			return Lecturer;
		}
		
		public IPagedList<ApplicationUserViewModel> GetStudents(IPageListModel<ApplicationUserViewModel> model, int page)
		{
			var query = db.ApplicationUsers.Where(p => !p.IsDeactivated).Include(x => x.Gender).AsQueryable();

			if (!query.Any())
			{
				return new List<ApplicationUserViewModel>().ToPagedList(page, 25);
			}
			query = query.Where(v => db.UserRoles
					.Any(ur => ur.UserId == v.Id && ur.RoleId == LMSConstants.StudentRoleId));
			if (!string.IsNullOrEmpty(model.Keyword))
			{
				query = query.Where(v =>
						v.FirstName.ToLower().Contains(model.Keyword.ToLower()) ||
						v.Email.ToLower().Contains(model.Keyword.ToLower()) ||
						v.LastName.ToLower().Contains(model.Keyword.ToLower()) ||
						v.Department.Name.ToLower().Contains(model.Keyword.ToLower()) ||
						v.Level.Name.ToLower().Contains(model.Keyword.ToLower()) ||
						v.PhoneNumber.Contains(model.Keyword.ToLower()));
			}
			var students = query
				.OrderByDescending(v => v.DateCreated)
				.Select(v => new ApplicationUserViewModel
				{
					Id = v.Id,
					FullName = v.FullName,
					PhoneNumber = v.PhoneNumber,
					Email = v.Email,
					Department = v.Department.Name,
					Level = v.Level.Name,
				})
				.ToPagedList(page, 25);
			model.Model = students;
			return students;

		}

	}
}
