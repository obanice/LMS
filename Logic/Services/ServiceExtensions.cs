using Core.Db;
using Core.Models;
using Hangfire;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.States;
using Hangfire.Storage;
using Logic;
using Logic.Helpers;
using Logic.IHelpers;
using Logic.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

public static class ServiceExtensions
{
	public static IServiceCollection RegisterHelpers(this IServiceCollection services)
	{
		services.AddScoped<IUserHelper, UserHelper>();
		services.AddScoped<IAdminHelper, AdminHelper>();
		services.AddScoped<IDropDownHelper, DropDownHelper>();
		services.AddScoped<ISuperAdminHelper, SuperAdminHelper>();
		services.AddScoped<IEmailHelper, EmailHelper>();
		services.AddScoped<IEmailService, EmailService>();

		return services;
	}

	public static IServiceCollection ConfigureAppSettings(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddSingleton<IEmailConfiguration>(configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>());
		//services.AddSingleton<IGeneralConfiguration>(configuration.GetSection("GeneralConfiguration").Get<GeneralConfiguration>());

		return services;
	}

	public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
	{
		services.AddIdentity<ApplicationUser, IdentityRole>(options =>
		{
			options.Password.RequireDigit = false;
			options.Password.RequiredLength = 3;
			options.Password.RequiredUniqueChars = 0;
			options.Password.RequireLowercase = false;
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequireUppercase = false;
		}).AddEntityFrameworkStores<AppDbContext>();

		return services;
	}

	public static IServiceCollection ConfigureFormOptions(this IServiceCollection services)
	{
		services.Configure<FormOptions>(x =>
		{
			x.ValueLengthLimit = int.MaxValue;
			x.MultipartBodyLengthLimit = int.MaxValue;
			x.MultipartHeadersLengthLimit = int.MaxValue;
		});

		return services;
	}

	public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
	{
		services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(options =>
			{
				options.Cookie.Name = "LMS.Auth";
				options.ExpireTimeSpan = TimeSpan.FromDays(30);
				options.SlidingExpiration = true;
			});

		return services;
	}

	public static IServiceCollection ConfigureForwardedHeaders(this IServiceCollection services)
	{
		services.Configure<ForwardedHeadersOptions>(options =>
		{
			options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
		});

		return services;
	}

	public class ExpirationTimeAttribute : JobFilterAttribute, IApplyStateFilter
	{
		public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
		{
			context.JobExpirationTimeout = TimeSpan.FromDays(30);
		}

		public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
		{
			context.JobExpirationTimeout = TimeSpan.FromDays(30);
		}
	}

	public class MyAuthorizationFilter : IDashboardAuthorizationFilter
	{
		public bool Authorize(DashboardContext context)
		{
			var user = context.GetHttpContext().User;
			return user != null && user.Identity.IsAuthenticated && user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "SuperAdmin");
		}
	}

	public static IApplicationBuilder UseHangFireConfiguration(this IApplicationBuilder app)
	{
		var dashboardOptions = new DashboardOptions
		{
			Authorization = new[] { new MyAuthorizationFilter() }
		};
        AppHttpContext.Services = app.ApplicationServices;
        app.UseHangfireDashboard("/LMSHangFire", dashboardOptions);

		var connectionString = app.ApplicationServices.GetService<IConfiguration>().GetConnectionString("HangfireConnectionString");

		GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);

		JobStorage.Current = JobStorage.Current;

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
              name: "areas",
              pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );
        });
        return app;
	}


	public static async Task SeedScreensAsync(AppDbContext context)
	{
		var existingUrls = await context.Screens.Select(s => s.Url).ToListAsync();

		existingUrls = existingUrls ?? new List<string>();

		var screens = new[]
		{
			//VHC Areas
			new { Url = "ScreenUrls.VHCDashboard", Class = "la la-dashboard", Name = "VHC Dashboard" },
			
		};

		var newScreens = screens.Where(s => !existingUrls.Contains(s.Url)).Select(s => new Screen
		{
			Url = s.Url,
			Class = s.Class,
			Name = s.Name,
			DateCreated = DateTime.UtcNow,
			Active = true
		}).ToList();

		if (newScreens.Any())
		{
			context.Screens.AddRange(newScreens);
			await context.SaveChangesAsync();
		}
	}
	public static async Task SeedRolesAsync(AppDbContext context)
	{
		var existingRoles = await context.Roles.Select(s => s.Id).ToListAsync();

		existingRoles = existingRoles ?? new List<string>();
		var roles = new[]
		{
			new { Id = Guid.Parse("0DB45C30-2FEE-47C6-AF34-7849A62B8856"), Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
			new { Id = Guid.Parse("4e3b804b-59a8-49d0-bf8b-2c71e46e7921"), Name = "Admin", NormalizedName = "ADMIN" },
			new { Id = Guid.Parse("06b353c6-37e8-4082-81b2-306236b9fc44"), Name = "Lecturer", NormalizedName = "LECTURER" },
			new { Id = Guid.Parse("4121dc45-10c6-41ab-b2dc-e990cc32a1ba"), Name = "Student", NormalizedName = "STUDENT" }
		};
		var newRoles = roles.Where(s => !existingRoles.Contains(s.Id.ToString())).Select(r => new IdentityRole
		{
			Id = r.Id.ToString(),
			Name = r.Name,
			NormalizedName = r.NormalizedName
		}).ToList();

		if (newRoles.Any())
		{
			context.Roles.AddRange(newRoles);
			await context.SaveChangesAsync();
		}
	}
	public static async Task SeedSuperAdminAsync(AppDbContext context)
	{
		var users = new[]
		{
			new ApplicationUser
			{
				Id = "65de30ed-e458-4557-a1ac-dcdee04d8660",
				UserName = "yahkwulu.o@gmail.com",
				NormalizedUserName = "YAHKWULU.O@GMAIL.COM",
                Email = "yahkwulu.o@gmail.com",
				NormalizedEmail = "YAHKWULU.O@GMAIL.COM",
				EmailConfirmed = false,
				PasswordHash = "AQAAAAEAACcQAAAAEGvS4F+i+YcLwaDUDfXxcEVXctzMzVCYRluNrIz29kKmkQAA2+5nvoHI5b7VXi7s+A==",
				SecurityStamp = "WTCMSYBIMEOURUJPG727IBVIJQ645ZJN",
				ConcurrencyStamp = "17656c44-f9a1-4e99-be79-54d7cd1e9d73",
				PhoneNumber = null,
				PhoneNumberConfirmed = false,
				TwoFactorEnabled = false,
				LockoutEnabled = true,
				AccessFailedCount = 0,
				MiddleName = null,
				DateCreated = DateTime.Now,
				IsDeactivated = false,
			}
		};

		foreach (var user in users)
		{
			if (!await context.Users.AnyAsync(u => u.Id == user.Id))
			{
				context.Users.Add(user);
			}
		}
		await context.SaveChangesAsync();
	}

	public static async Task SeedSuperAdminRolesAsync(AppDbContext context)
	{
		var userRoles = new[]
		{
			new { UserId = Guid.Parse("65de30ed-e458-4557-a1ac-dcdee04d8660"), RoleId = Guid.Parse("0DB45C30-2FEE-47C6-AF34-7849A62B8856") }
		};

		foreach (var userRole in userRoles)
		{
			if (!await context.UserRoles.AnyAsync(ur => ur.UserId == userRole.UserId.ToString() && ur.RoleId == userRole.RoleId.ToString()))
			{
				context.UserRoles.Add(new IdentityUserRole<string>
				{
					UserId = userRole.UserId.ToString(),
					RoleId = userRole.RoleId.ToString()
				});
			}
		}

		await context.SaveChangesAsync();
	}
	public static async Task SeedCommonDropDownsAsync(AppDbContext context)
	{
		var existingDropdowns = await context.CommonDropDowns.Select(s => s.Name).ToListAsync();

		existingDropdowns = existingDropdowns ?? new List<string>();
		var dropdowns = new[]
		{
			new { DropdownKey = 1, Name = "Male"},
			new { DropdownKey = 1, Name = "Female"},
			new { DropdownKey = 1, Name = "Prefer not to say"}
		};
		var newDropdowns = dropdowns.Where(s => !existingDropdowns.Contains(s.Name)).Select(d => new CommonDropDown
		{
			Name = d.Name,
			DropDownKey = d.DropdownKey
		}).ToList();

		if (newDropdowns.Any())
		{
			context.AddRange(newDropdowns);
			await context.SaveChangesAsync();
		}
	}
}
