using Core.Db;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace LMS.Controllers
{
	public abstract class BaseController : Controller
	{
		public BaseController() { }

		private ApplicationUser _user;

		public ApplicationUser? CurrentUser
		{
			get
			{
				return Utility.GetCurrentUser();
			}
		}

		public string? CurrentUserId => CurrentUser?.Id;
		public string? CurrentUserName => CurrentUser?.FullName;
		public int? CurrentUserDepartmentId => CurrentUser?.DepartmentId;
		
		public void LogCritical(string message)
		{
			Log.Logger.Fatal(message);
		}

		public void LogError(string error)
		{
			Log.Logger.Error(error);
		}

		public void LogInformation(string error)
		{
			Log.Logger.Information(error);
		}

		public void LogVerbose(string error)
		{
			Log.Logger.Verbose(error);
		}

		public void LogWarning(string error)
		{
			Log.Logger.Warning(error);
		}

	}
}
 