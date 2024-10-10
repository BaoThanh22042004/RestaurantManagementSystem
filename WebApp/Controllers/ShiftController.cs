using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories;
using Repositories.Interface;
using WebApp.Models;

namespace WebApp.Controllers
{
	[Route("Dashboard/Shift")]
	[Authorize(Roles = "Manager")]
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

		[Route("Create")]
		public IActionResult Create()
		{
			return View("CreateShiftView");
		}

		[Route("Create")]
		[HttpPost]
		public async Task<IActionResult> Create(ShiftViewModel shiftViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View("CreateShiftView", shiftViewModel);
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
				return View("CreateShiftView", shiftViewModel);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while creating shift. Please try again later.";
				return View("CreateShiftView", shiftViewModel);
			}

			return RedirectToAction("Index");
		}

		[Route("Details/{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var shift = await _shiftRepository.GetByIDAsync(id);
			if (shift == null)
			{
				return NotFound();
			}
			var shiftViewModel = new ShiftViewModel(shift);
			return View("DetailsShiftView", shiftViewModel);
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
			return View("EditShiftView", shiftViewModel);
		}

		[HttpPost]
		[Route("Edit/{ShiftId}")]
		public async Task<IActionResult> Edit(ShiftViewModel shift, int ShiftId)
		{
			if (!ModelState.IsValid)
			{
				return View("EditShiftView", shift);
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
				return View("EditShiftView", shift);
			}

			return RedirectToAction("Index");
		}

		[Route("Shift/Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var shift = await _shiftRepository.GetByIDAsync(id);
			if (shift == null)
			{
				return NotFound();
			}

			var shiftViewModel = new ShiftViewModel(shift);
			return View("DeleteShiftView", shiftViewModel);
		}

		[HttpPost]
		[Route("Shift/Delete/{ShiftId}")]
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

			return RedirectToAction("Index");
		}
	}
}
