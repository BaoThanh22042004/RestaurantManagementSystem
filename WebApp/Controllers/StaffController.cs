using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
	[Route("Dashboard/Staff")]
	[Authorize(Roles = "Manager")]
	public class StaffController : Controller
	{
		public IActionResult Index()
		{
			return View("Staff");
		}
	}
}
