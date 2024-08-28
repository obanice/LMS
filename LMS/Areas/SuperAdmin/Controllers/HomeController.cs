using Microsoft.AspNetCore.Mvc;

namespace LMS.Areas.SuperAdmin.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
