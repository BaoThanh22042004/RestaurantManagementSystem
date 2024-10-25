using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories;
using Repositories.Interface;
using WebApp.Models;

namespace WebApp.Controllers
{
	[Route("Dashboard/Shift")]
	[Authorize(Roles = $"{nameof(Role.Manager)}")]
	public class ShiftController : Controller
	{
		private readonly IShiftRepository _shiftRepository;

		public ShiftController(IShiftRepository shiftRepository)
		{
			_shiftRepository = shiftRepository;
		}

		public async Task<IActionResult> Index()
		{
			var shifts = await _shiftRepository.GetAllAsync();
			var shiftList = shifts.Select(shift => new ShiftViewModel(shift));
			return View("ShiftView", shiftList);
		}

        [HttpGet("Create")]
        public IActionResult Create()
		{
			return PartialView("_CreateShiftModal");
		}

        [HttpPost("Create")]
        public async Task<IActionResult> Create(ShiftViewModel shiftViewModel)
		{
			
			if (!ModelState.IsValid)
			{
				return PartialView("_CreateShiftModal", shiftViewModel);
			}

			try
			{
				var shift = new Shift
				{
					ShiftName = shiftViewModel.ShiftName,
					StartTime = shiftViewModel.StartTime,
					EndTime = shiftViewModel.EndTime
				};

				await _shiftRepository.InsertAsync(shift);
			}
			catch (ArgumentException e)
			{
				TempData["Error"] = e.Message;
				return PartialView("_CreateShiftModal", shiftViewModel);
            }
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while creating shift. Please try again later.";
				return PartialView("_CreateShiftModal", shiftViewModel);
            }

			return Json(new { success = true });
        }

		[HttpGet("Details/{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var shift = await _shiftRepository.GetByIDAsync(id);
			if (shift == null)
			{
				return NotFound();
			}
			var shiftViewModel = new ShiftViewModel(shift);
			return PartialView("_DetailsShiftModal", shiftViewModel);
		}

		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var shift = await _shiftRepository.GetByIDAsync(id);
			if (shift == null)
			{
				return NotFound();
			}

			var shiftViewModel = new ShiftViewModel(shift);
			return PartialView("_EditShiftModal", shiftViewModel);
		}

		[HttpPost]
		[Route("Edit/{ShiftId}")]
		public async Task<IActionResult> Edit(ShiftViewModel shift, int ShiftId)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_EditShiftModal", shift);
			}

			try
			{
				var shiftEntity = await _shiftRepository.GetByIDAsync(ShiftId);
				if (shiftEntity == null)
				{
					return NotFound();
				}

				shiftEntity.ShiftName = shift.ShiftName;
				shiftEntity.StartTime = shift.StartTime;
				shiftEntity.EndTime = shift.EndTime;

				await _shiftRepository.UpdateAsync(shiftEntity);
			}
			catch (InvalidOperationException)
			{
				return NotFound();
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while updating shift. Please try again later.";
				return PartialView("_EditShiftModal", shift);
			}

			return Json(new { success = true });
        }

		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var shift = await _shiftRepository.GetByIDAsync(id);
			if (shift == null)
			{
				return NotFound();
			}

			var shiftViewModel = new ShiftViewModel(shift);
			return PartialView("_DeleteShiftModal", shiftViewModel);
		}

		[HttpPost]
		[Route("Delete/{ShiftId}")]
		public async Task<IActionResult> DeleteConfirmed(int ShiftId)
		{
			try
			{
				await _shiftRepository.DeleteAsync(ShiftId);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while deleting shift. Please try again later.";
				return RedirectToAction("Delete", new { ShiftId });
			}

			return Json(new { success = true });
        }
	}
}
