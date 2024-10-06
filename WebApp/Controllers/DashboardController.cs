using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
	[Authorize(Policy = "Staff")]
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{
			return View("Dashboard");
		}
	}
}
