using Core.Db;
using Core.Models;
using Core.ViewModels;
using LMS.Models;
using Logic.Helpers;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace LMS.Areas.Security.Controllers
{
	[Area("Security")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        private readonly AppDbContext db;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            IUserHelper userHelper, 
            SignInManager<ApplicationUser> signInManager,
            AppDbContext dbContext)
        {
            _userManager = userManager;
            _userHelper = userHelper;
            _signInManager = signInManager;
            db = dbContext;
        }
        [HttpGet]
        public IActionResult Register()
        {
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

                var url = UserHelper.GetValidatedUrl();
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
                if (userDetails == null)
                {
                    return ResponseHelper.JsonError("An error has occurred, try again. Please contact support if the error persists");
                }
                var applicationUserViewModel = JsonConvert.DeserializeObject<ApplicationUserViewModel>(userDetails);
                if (applicationUserViewModel != null)
                {
                    return ResponseHelper.JsonError("Error occurred");
                }
                var checkForEmail = await _userHelper.FindByEmailAsync(applicationUserViewModel.Email).ConfigureAwait(false);
                if (checkForEmail != null)
                {
                    return Json(new { isError = true, msg = "Email belongs to another user" });
                }
                if (applicationUserViewModel.Password != applicationUserViewModel.ConfirmPassword)
                {
                    return Json(new { isError = true, msg = "Password and Confirm password must match" });
                }
                var isUserCreated = await _userHelper.CreateUser(applicationUserViewModel).ConfigureAwait(false);
                if (isUserCreated)
                {
                    return Json(new { isError = false, msg = "Registered successfully, login to continue" });
                }
                return Json(new { isError = true, msg = "Unable to register" });
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal($"An attempt to register in user failed with this exception: {ex.Message ?? ex.InnerException?.Message}");
                throw;
            }
        }
    }
}
