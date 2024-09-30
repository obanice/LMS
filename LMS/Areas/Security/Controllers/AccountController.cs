using Core.Db;
using Core.Models;
using Core.ViewModels;
using LMS.Models;
using Logic.Helpers;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using static Core.Enums.LMSEnum;

namespace LMS.Areas.Security.Controllers
{
	[Area("Security")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly IEmailHelper emailHelper;
        private readonly AppDbContext db;
        private readonly IDropDownHelper _dropDownHelper;
		public AccountController(
			UserManager<ApplicationUser> userManager,
			IUserHelper userHelper,
			SignInManager<ApplicationUser> signInManager,
			AppDbContext dbContext,
			IEmailHelper emailHelper,
            IDropDownHelper dropDownHelper)
		{
			_userManager = userManager;
			_userHelper = userHelper;
			_signInManager = signInManager;
			db = dbContext;
			this.emailHelper = emailHelper;
            _dropDownHelper = dropDownHelper;
		}
		[HttpGet]
        public IActionResult Register()
        {
            ViewBag.Gender = _dropDownHelper.GetDropDownByKey(DropDownEnums.Gender);
            ViewBag.Department = _dropDownHelper.GetDepartments();
            ViewBag.Level = _dropDownHelper.GetDropDownByKey(DropDownEnums.Level);
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Login(string emailorphone, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailorphone) || string.IsNullOrWhiteSpace(password))
                {
                    return ResponseHelper.JsonError("Please fill the form correctly");
                }
                var filterSpace = emailorphone.Replace(" ", "");
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var user = await _userHelper.FindByEmailAsync(filterSpace).ConfigureAwait(false);
                if (user == null)
                {
                    user = await _userHelper.FindByPhoneNumber(filterSpace).ConfigureAwait(false);
                    if (user == null)
                    {
                        return ResponseHelper.JsonError("Invalid detail or account does not exist, contact your Admin");
                    }
                }

                var result = await _signInManager.PasswordSignInAsync(user, password, true, lockoutOnFailure: false).ConfigureAwait(false);

                if (!result.Succeeded)
                {
                    return ResponseHelper.JsonError("Invalid user name or password");
                }
                user.Roles = (List<string>)await _userManager.GetRolesAsync(user).ConfigureAwait(false);

                user.UserRole = user.Roles.Contains(Utility.Constants.SuperAdminRole) ? Utility.Constants.SuperAdminRole :
                                user.Roles.Contains(Utility.Constants.AdminRole) ? Utility.Constants.AdminRole :
                                user.Roles.Contains(Utility.Constants.LecturerRole) ? Utility.Constants.LecturerRole :
                                Utility.Constants.StudentRole;

                user.RoleId = db.UserRoles.FirstOrDefault(x => x.UserId == user.Id)?.RoleId;

                //Stored User to session
                var currentUser = JsonConvert.SerializeObject(user, settings);
                HttpContext.Session.SetString("loggedInUser", currentUser);

                var url = _userHelper.GetValidatedUrl();
                return Json(new { isError = false, dashboard = url });
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal($"An attempt to login in user failed with this exception: {ex.Message ?? ex.InnerException?.Message}");
                throw;
            }
        }

        [HttpPost]
        public async Task<JsonResult> Register(string userDetails)
        {
            try
            {
                if (string.IsNullOrEmpty(userDetails))
                {
                    return ResponseHelper.JsonError("An error has occurred, try again. Please contact support if the error persists");
                }
                var applicationUserViewModel = JsonConvert.DeserializeObject<ApplicationUserViewModel>(userDetails);
                if (applicationUserViewModel == null)
                {
                    return ResponseHelper.JsonError("Error occurred");
                }
                var checkForEmail = await _userHelper.FindByEmailAsync(applicationUserViewModel.Email).ConfigureAwait(false);
                if (checkForEmail != null)
                {
					return ResponseHelper.JsonError("Email belongs to another user");
                }
                if (applicationUserViewModel.Password != applicationUserViewModel.ConfirmPassword)
                {
					return ResponseHelper.JsonError("Password and Confirm password must match");
                }
                var isUserCreated = await _userHelper.CreateUser(applicationUserViewModel).ConfigureAwait(false);
                if (isUserCreated)
                {
					return ResponseHelper.JsonSuccess("Registered successfully, login to continue");
                }
                return ResponseHelper.JsonError("Unable to register");
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal($"An attempt to register in user failed with this exception: {ex.Message ?? ex.InnerException?.Message}");
                throw;
            }
        }

		[HttpGet]
		public IActionResult ResetPassword(Guid token)
		{
			if (token != Guid.Empty)
			{
				//var passwordResetViewmodel = new HomePageDto()
				//{
				//	Token = token,
				//};
				//return View(passwordResetViewmodel);
			}
			return View();
		}

		[HttpPost]
		public async Task<JsonResult> ResetPassword(string passwordResetViewmodel)
		{
			var userdetail = JsonConvert.DeserializeObject<PasswordResetViewmodel>(passwordResetViewmodel);
			if (userdetail != null)
			{
				if (userdetail.Password != userdetail.ConfirmPassword)
				{
					return Json(new { isError = true, msg = "Password and confirm password must match" });
				}
				var userVerification = await emailHelper.GetUserToken(userdetail.Token).ConfigureAwait(false);
				if (userVerification != null && !userVerification.Used)
				{
					await _userManager.RemovePasswordAsync(userVerification.User).ConfigureAwait(false);
					await _userManager.AddPasswordAsync(userVerification.User, userdetail.Password).ConfigureAwait(false);
					await db.SaveChangesAsync().ConfigureAwait(false);
					await emailHelper.MarkTokenAsUsed(userVerification).ConfigureAwait(false);

					//var sendEmail = _emailHelper.PasswordResetConfirmation(userVerification.User);
					//if (sendEmail)
					//{
					//	return Json(new { isError = false, msg = "Your Password has been reset successfully, you can now use the new password on your next login" });
					//}
					return Json(new { isError = true, mgs = "Sorry! The Link You Entered is Invalid or Expired " });
				}
			}
			return Json(new { isError = true, mgs = "An error has occurred, try again. Please contact support if the error persists." });
		}
		[HttpPost]
		public async Task<IActionResult> LogOut()
		{
			await _signInManager.SignOutAsync();
			HttpContext.Session.Clear();
			return RedirectToAction("Login", "Account", new { area = "Security" });
		}
	}
}
