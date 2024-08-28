using Core.Db;
using Hangfire;
using Logic.Helpers.ExceptionFolder;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Serilog;
using System.Net;
using static ServiceExtensions;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging
builder.Host.UseSerilog((context, configuration) => configuration
	.ReadFrom.Configuration(context.Configuration)
	.WriteTo.Console()
	.WriteTo.File("Logs\\log-.txt", rollingInterval: RollingInterval.Day)
	.Enrich.FromLogContext());

// Add services to the container
builder.Services.AddControllersWithViews(options =>
{
	options.Filters.Add<CustomExceptionFilterAttribute>();
});

builder.Services.RegisterHelpers();
builder.Services.ConfigureAppSettings(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureFormOptions();
builder.Services.ConfigureAuthentication();
builder.Services.ConfigureForwardedHeaders();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnectionString")));
builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnectionString")));
GlobalJobFilters.Filters.Add(new ExpirationTimeAttribute());
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.IdleTimeout = TimeSpan.FromMinutes(70000);
});


ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseDeveloperExceptionPage();
}

app.UseStatusCodePages(async context =>
{
	var response = context.HttpContext.Response;
	if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
	{
		response.Redirect("/Security/Account/Login");
	}
	else if (response.StatusCode == (int)HttpStatusCode.NotFound ||
		response.StatusCode == (int)HttpStatusCode.Forbidden ||
		response.StatusCode == (int)HttpStatusCode.BadGateway ||
		response.StatusCode == (int)HttpStatusCode.InternalServerError)
	{
		response.Redirect("/Home/Error");
	}
});

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseSession();
app.UseRouting();
UpdateDatabase(app);
app.UseAuthorization();
app.UseHangFireConfiguration();

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllerRoute(
	  name: "areas",
	  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
	);
});

app.MapControllerRoute(
		name: "default",
		pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static void UpdateDatabase(IApplicationBuilder app)
{
	using var serviceScope = app.ApplicationServices
		.GetRequiredService<IServiceScopeFactory>()
		.CreateScope();
	var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

	if (context != null)
	{
		context.Database.Migrate();

		ServiceExtensions.SeedRolesAsync(context).Wait();
		ServiceExtensions.SeedSuperAdminAsync(context).Wait();
		ServiceExtensions.SeedSuperAdminRolesAsync(context).Wait();
		ServiceExtensions.SeedScreensAsync(context).Wait();
	}
}
