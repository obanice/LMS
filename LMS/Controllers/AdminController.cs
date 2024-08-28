using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
	public class AdminController : BaseController
	{

		public IActionResult Index()
		{
			return View();
		}
	}
}
