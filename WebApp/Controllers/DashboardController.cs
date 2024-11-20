using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Repositories.Interface;

namespace WebApp.Controllers
{
	[Authorize(Policy = "Staff")]
	public class DashboardController : Controller
	{
		private readonly IFinancialReportRepository _financialReportRepository;

		public DashboardController(IFinancialReportRepository financialReportRepository)
		{
			_financialReportRepository = financialReportRepository;
		}

		public async Task<IActionResult> Index()
		{
			if (User.IsInRole("Manager") || User.IsInRole("Accountant"))
			{
				var financialReports = await _financialReportRepository.GetAllAsync();
				return View("DashboardView", financialReports);
			}
			return View("DashboardView", null);
		}
	}
}
