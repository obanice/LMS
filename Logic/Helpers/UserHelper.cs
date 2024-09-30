using Core.Constants;
using Core.Db;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Logic.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserHelper(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<ApplicationUser>? FindByEmailAsync(string email)
        {
            return await _context.ApplicationUsers
                .Where(s => s.Email == email && !s.IsDeactivated)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }
        public async Task<ApplicationUser>? FindByPhoneNumber(string phone)
        {
            return await _context.ApplicationUsers
                .Where(s => s.PhoneNumber == phone && !s.IsDeactivated)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }
        public async Task<ApplicationUser> UpdateSession(string? userName)
        {
			var settings = new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};
			var user = await FindByEmailAsync(userName).ConfigureAwait(false);
			if (user == null)
			{
				user = await FindByPhoneNumber(userName).ConfigureAwait(false);
			}
			user.Roles = (List<string>)await _userManager.GetRolesAsync(user).ConfigureAwait(false);

			user.UserRole = user.Roles.Contains(Utility.Constants.SuperAdminRole) ? Utility.Constants.SuperAdminRole :
							user.Roles.Contains(Utility.Constants.AdminRole) ? Utility.Constants.AdminRole :
							user.Roles.Contains(Utility.Constants.LecturerRole) ? Utility.Constants.LecturerRole :
							Utility.Constants.StudentRole;

			user.RoleId = _context.UserRoles.FirstOrDefault(x => x.UserId == user.Id)?.RoleId;

			//Stored User to session
			var currentUser = JsonConvert.SerializeObject(user, settings);
			AppHttpContext.Current.Session.SetString("loggedInUser", currentUser);
            return user;
		}
        public string GetValidatedUrl()
        {
            var loggedInUser = Utility.GetCurrentUser();
            var roleUrlMap = new Dictionary<string, string>
            {
                { Utility.Constants.SuperAdminRole, LMSConstants.SuperAdminDashboard },
                { Utility.Constants.LecturerRole, LMSConstants.LecturerDashboard },
                { Utility.Constants.AdminRole, LMSConstants.Dashboard },
                { Utility.Constants.StudentRole, LMSConstants.StudentDashboard }
            };
            if (loggedInUser.DepartmentId == null || loggedInUser.DepartmentId == 0)
            {
               loggedInUser = UpdateSession(loggedInUser.UserName).Result;
			}
            foreach (var role in loggedInUser.Roles)
            {
                if (roleUrlMap.TryGetValue(role, out var url))
                {
                    return url;
                }
            }
            return "/Security/Account/Login";
        }

        public async Task<bool> CreateUser(ApplicationUserViewModel userViewModel)
        {
            var user = new ApplicationUser();
            user.UserName = userViewModel.Email;
            user.Email = userViewModel.Email;
            user.FirstName = userViewModel.FirstName;
            user.LastName = userViewModel.LastName;
            user.PhoneNumber = userViewModel.PhoneNumber;
            user.DateCreated = DateTime.Now;
            user.IsDeactivated = false;
			user.LevelId = userViewModel.LevelId;
			user.DepartmentId = userViewModel.DepartmentId;
            var createUser = await _userManager.CreateAsync(user, userViewModel.Password).ConfigureAwait(false);
            if (createUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Utility.Constants.StudentRole).ConfigureAwait(false);
                return true;
            }
            return false;
        }
		public static string GetRoleLayout()
		{
			var loggedInUser = Utility.GetCurrentUser();
			if (loggedInUser == null)
			{
				return Utility.Constants.DefaultLayout;
			}
			var isSuperAdmin = loggedInUser.Roles.Contains(Utility.Constants.SuperAdminRole);
			return isSuperAdmin ? Utility.Constants.SuperAdminLayout : Utility.Constants.GeneralLayout;
		}

	}
}
