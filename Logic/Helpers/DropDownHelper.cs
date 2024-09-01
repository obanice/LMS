using Core.Constants;
using Core.Db;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.EntityFrameworkCore;
using static Core.Enum.LMSEnum;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Logic.Helpers
{
	public class DropDownHelper : IDropDownHelper
	{
		private readonly AppDbContext _context;
		public DropDownHelper(AppDbContext context)
		{
			_context = context;
		}


		

		public class DropdownEnumModel
		{
			public int Id { get; set; }
			public string? Name { get; set; }
		}

		public List<CommonDropDown> GetDropDownByKey(DropDownEnums dropDownKey)
		{
			var common = new CommonDropDown()
			{
				Id = 0,
				Name = "-- Select --"
			};
			var dropdowns =  _context.CommonDropDowns
				.Where(s => s.Active && s.DropDownKey == (int)dropDownKey)
				.OrderBy(s => s.Name)
				.ToList();
			dropdowns.Insert(0, common);
			return dropdowns;
		}

		public List<DropdownEnumModel> GetDropDownEnumList()
		{
			var common = new DropdownEnumModel()
			{
				Id = 0,
				Name = "-- Select --"

			};
			var enumList = ((DropDownEnums[])Enum.GetValues(typeof(DropDownEnums)))
				.Select(c => new DropdownEnumModel() { Id = (int)c, Name = c.ToString() })
				.ToList();
			enumList.Insert(0, common);
			return enumList;
		}


		public async Task<bool> CreateDropDownsAsync(CommonDropDown commonDropDowns)
		{
			if (commonDropDowns != null && commonDropDowns.DropDownKey > 0 && commonDropDowns.Name != null)
			{
				var newCommonDropdowns = new CommonDropDown
				{
					Name = commonDropDowns.Name,
					DropDownKey = commonDropDowns.DropDownKey
				};

				var createdDropdowns = await _context.AddAsync(newCommonDropdowns);
				await _context.SaveChangesAsync();
				if (createdDropdowns.Entity.Id > 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool EditDropDown(CommonDropDown commonDropdowns)
		{
			var dropdown = _context.CommonDropDowns.Where(c => c.Id == commonDropdowns.Id).FirstOrDefault();
			if (dropdown == null)
			{
				return false;
			}
			dropdown.Name = commonDropdowns.Name;
			_context.Update(dropdown);
			_context.SaveChanges();

			return true;
		}

		public DropdownEnumModel GetEnumById(int enumkeyId)
		{
			return ((DropDownEnums[])Enum.GetValues(typeof(DropDownEnums)))
				.Select(c => new DropdownEnumModel() { Id = (int)c, Name = c.ToString() })
				.Where(x => x.Id == enumkeyId)
				.FirstOrDefault();
		}

		public bool DeleteDropDownById(int id)
		{
			if (id == 0)
			{
				return false;
			}
			var dropdown = _context.CommonDropDowns.Where(d => d.Id == id && d.Active && d.Active).FirstOrDefault();
			if (dropdown == null)
			{
				return false;
			}
			dropdown.Active = false;
			_context.CommonDropDowns.Update(dropdown);
			_context.SaveChanges();
			return true;
		}

		public List<ApplicationUserViewModel> GetUsers()
		{
			var common = new ApplicationUserViewModel()
			{
				Id = "",
				DropDownName = "-- Select --"
			};
			var users = _context.ApplicationUsers.Where(x => x.Id != null  && x.IsDeactivated).Select(x => new ApplicationUserViewModel
			{
				Id = x.Id,
				DropDownName = x.FullName,
			}).ToList();
			users.Insert(0, common);
			return users;
		}
		public List<DepartmentViewModel> GetDepartments()
		{
			var common = new DepartmentViewModel()
			{
				Id = 0,
				Name = "-- Select --"
			};
			var schools = _context.Departments
				.Where(x => x.Active)
				.Select(x => new DepartmentViewModel
				{
					Id = x.Id,
					Name = x.Name,
				}).ToList();
			schools.Insert(0, common);
			return schools;
		}
		public List<ApplicationUserViewModel> GetLecturers()
		{
			var common = new ApplicationUserViewModel()
			{
				Id = "",
				FullName = "-- Select --"
			};
			var users = _context.ApplicationUsers
				.Where(x => !x.IsDeactivated && _context.UserRoles
					.Any(ur => ur.UserId == x.Id && ur.RoleId == LMSConstants.LecturerRoleId))
				.Select(x => new ApplicationUserViewModel
				{
					Id = x.Id,
					FullName = x.FullName,
				})
				.ToList();
			users.Insert(0, common);

			return users;
		}

	}
	public class drp
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}
}
