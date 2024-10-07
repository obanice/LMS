using Core;
using Core.Constants;
using Core.Db;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using X.PagedList;
using X.PagedList.Extensions;

namespace Logic.Helpers
{
	public class AdminHelper :BaseHelper, IAdminHelper
	{
		private readonly AppDbContext db;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IStudentHelper _studentHelper;
		public AdminHelper(AppDbContext dbContext, 
			UserManager<ApplicationUser> userManager,
			IStudentHelper studentHelper):base(dbContext)
			{
				db = dbContext;
				_userManager = userManager;
			_studentHelper = studentHelper;
			}
		public IPagedList<LecturerViewModel> Lectures(IPageListModel<LecturerViewModel> model, int? departmentId, int page)
		{
			var query = GetByPredicate<ApplicationUser>(p => !p.IsDeactivated && p.DepartmentId == departmentId)
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
		public IPagedList<CourseViewModel> Courses(IPageListModel<CourseViewModel> model, int? departmentId, string lecturerId, int page)
		{
			IQueryable<Course> query = GetByPredicate<Course>(p => p.Active);

			//lecturerId = "410067eb-617d-4669-ab97-d64f7c8eacf0";

			if (departmentId.HasValue && departmentId != 0)
			{
				query = query.Where(p => p.DepartmentId == departmentId);
			}

			if (!string.IsNullOrEmpty(lecturerId))
			{
				query = query.Where(p => p.LecturerId == lecturerId);
			}

			if (!string.IsNullOrEmpty(model.Keyword))
			{
				var keyword = model.Keyword.ToLower();
				query = query.Where(v =>
						v.Code.ToLower().Contains(keyword) ||
						v.Name.ToLower().Contains(keyword) ||
						v.Description.ToLower().Contains(keyword) ||
						v.Lecturer.FirstName.ToLower().Contains(keyword) ||
						v.Department.Name.ToLower().Contains(keyword));
			}

			if (model.StartDate.HasValue)
			{
				query = query.Where(v => v.DateCreated >= model.StartDate);
			}

			if (model.EndDate.HasValue)
			{
				query = query.Where(v => v.DateCreated <= model.EndDate);
			}

			query = query.Include(x => x.Lecturer)
						 .Include(x => x.Department)
						 .Include(x => x.Semester)
						 .Include(x => x.Level)
						 .Include(x=>x.Quizzes);

			if (!query.Any())
			{
				return new List<CourseViewModel>().ToPagedList(page, 25);
			}

			var courses = query.OrderByDescending(v => v.DateCreated)
							   .Select(v => new CourseViewModel
							   {
								   Id = v.Id,
								   Name = v.Name,
								   Code = v.Code,
								   Description = v.Description,
								   LecturerName = v.Lecturer.FullName,
								   Department = v.Department.Name,
								   Semester = v.Semester.Name,
								   Level = v.Level.Name
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
				LevelId = courseViewModel.LevelId
			};
			db.Add(course);
			db.SaveChanges();
			return true;
		}
		public List<StudyMaterialViewModel> GetStudyMaterialsByCoursesById(int?  courseId)
		{
			return [.. GetByPredicate<StudyMaterial>(x => x.CourseId == courseId && x.Active)
				.Include(x => x.MediaType)
				.Include(x => x.Course)
				.Select(v => new StudyMaterialViewModel
				{
					Id = v.Id,
					CourseId = v.CourseId,
					Name = v.MediaType.Name,
					File = v.MediaType.PhysicalPath,
					Date = v.DateCreated.ToFormattedDate(),
					FileExtension = v.MediaType.MediaType.GetEnumDescription()
				})];
		}
		
		public AdminDashboardViewModel FetchAdminData(int? departmentId)
		{
			var dashboardViewModel = new AdminDashboardViewModel();
			dashboardViewModel.CoursesCount = CoursesByDepartmentId(departmentId).Count;
			dashboardViewModel.LecturerCount = _userManager.GetUsersInRoleAsync(Utility.Constants.LecturerRole).Result.Count;
			dashboardViewModel.StudentCount = _userManager.GetUsersInRoleAsync(Utility.Constants.StudentRole).Result.Count;
			return dashboardViewModel;
		}
		public List<CourseViewModel> CoursesByDepartmentId(int? departmentId)
		{

			return [..GetByPredicate<Course>(c => c.DepartmentId == departmentId && c.Active)
				.Include(t => t.Level)
				.Include(t => t.Semester)
				.Select(c => new CourseViewModel
				{
					Id = c.Id,
					Code = c.Code,
					Name = c.Name,
					Level = c.Level.Name,
					Semester = c.Semester.Name,

				}).ToList()];
		}
		public bool AddMaterial(int? courseId, int? mediaId)
		{
			var materia = new StudyMaterial
			{
				CourseId = courseId,
				MediaTypeId = mediaId,
			};
			return Create<StudyMaterial, StudyMaterial>(materia);
		}
        public bool DeleteMaterial(int? materialId)
        {
            var material = GetByPredicate<StudyMaterial>(x => x.Id == materialId && x.Active).FirstOrDefault();
			if (material == null)
			{
				return false;
			}
            var media = GetByPredicate<Media>(x => x.Id == material.MediaTypeId && x.Active).FirstOrDefault();
            if (media != null)
            {
                HardDelete<Media, int>(media.Id);
            }
            return HardDelete<StudyMaterial, int>(materialId.Value);
        }
    }
}
