using Core.Constants;
using Core.Db;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public static string GetValidatedUrl()
        {
            var loggedInUser = Utility.GetCurrentUser();
            var roleUrlMap = new Dictionary<string, string>
            {
                { Utility.Constants.SuperAdminRole, LMSConstants.SuperAdminDashboard },
                { Utility.Constants.LecturerRole, LMSConstants.LecturerDashboard },
                { Utility.Constants.AdminRole, LMSConstants.Dashboard },
                { Utility.Constants.StudentRole, LMSConstants.StudentDashboard }
            };

            foreach (var role in loggedInUser.Roles)
            {
                if (roleUrlMap.TryGetValue(role, out var url))
                {
                    return url;
                }
            }
            return "/Security/Account/Login";
        }

        public async Task<bool> CreateUser(ApplicationUserViewModel userDetails)
        {
            var user = new ApplicationUser();
            user.UserName = userDetails.Email;
            user.Email = userDetails.Email;
            user.FirstName = userDetails.FirstName;
            user.LastName = userDetails.LastName;
            user.PhoneNumber = userDetails.PhoneNumber;
            user.DateCreated = DateTime.Now;
            user.IsDeactivated = false;
            if (userDetails.Role == Utility.Constants.StudentRole)
            {
                user.Level = userDetails.Level;
            }
            user.DepartmentId = userDetails.DepartmentId;
            var createUser = await _userManager.CreateAsync(user, userDetails.Password).ConfigureAwait(false);
            if (createUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, userDetails.Role).ConfigureAwait(false);
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
			if (loggedInUser.Roles.Contains(Utility.Constants.SuperAdminRole))
			{
				return Utility.Constants.SuperAdminLayout;
			}

			if (loggedInUser.Roles.Contains(Utility.Constants.AdminRole))
			{
				return Utility.Constants.AdminLayout;
			}

			if (loggedInUser.Roles.Contains(Utility.Constants.LecturerRole))
			{
				return Utility.Constants.LecturerLayout;
			}
			return Utility.Constants.StudentLayout;
		}

	}
}
