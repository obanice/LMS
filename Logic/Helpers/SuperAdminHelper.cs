using Core.Db;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
	}
}
