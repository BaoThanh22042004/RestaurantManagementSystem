	using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Quartz;
using Repositories;
using Repositories.Interface;
using WebApp.Models;
using WebApp.Utilities;

namespace WebApp.Controllers
{
	[Route("Dashboard/Shift")]
	[Authorize(Roles = $"{nameof(Role.Manager)}")]
	public class ShiftController : Controller
	{
		private readonly IShiftRepository _shiftRepository;
		private readonly QuartzJobScheduler _scheduler;

		public ShiftController(IShiftRepository shiftRepository, QuartzJobScheduler scheduler)
		{
			_shiftRepository = shiftRepository;
			_scheduler = scheduler;
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
            async Task<IActionResult> InvalidView(string? error = null)
            {
                if (error != null)
                {
                    TempData["Error"] = error;
                }
                return PartialView("_CreateShiftModal", shiftViewModel);
            }

            if (!ModelState.IsValid)
			{
                return await InvalidView();
            }

			try
			{
				var shiftEntitys = await _shiftRepository.GetAllAsync();
				var startTime = shiftViewModel.StartTime;
				var endTime = shiftViewModel.EndTime;
                TimeSpan duration;
                duration = endTime - startTime;
                decimal workingHours = (decimal)duration.TotalHours;

                if (workingHours < 1) 
				{
                    return await InvalidView("Cannot create a Shift with the total hour smaller than one hour!");
                }

				if (shiftViewModel.EndTime < shiftViewModel.StartTime) 
				{
                    return await InvalidView("Cannot create a Shift with the start time greater than end time!");
                }

				foreach (Shift item in shiftEntitys) 
				{
                    if (item.StartTime == shiftViewModel.StartTime && item.EndTime == shiftViewModel.EndTime)
                    {
                        return await InvalidView("This Shift has already exists!");
                    }
                }				
				var shift = new Shift
				{
					ShiftName = shiftViewModel.ShiftName,
					StartTime = shiftViewModel.StartTime,
					EndTime = shiftViewModel.EndTime
				};

				var entity = await _shiftRepository.InsertAsync(shift);

				// Schedule the job
				await _scheduler.ScheduleCheckAttendanceJob(entity.ShiftId, entity.StartTime, entity.EndTime);
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
            async Task<IActionResult> InvalidView(string? error = null)
            {
                if (error != null)
                {
                    TempData["Error"] = error;
                }
                return PartialView("_EditShiftModal", shift);
            }

            if (!ModelState.IsValid)
			{
				return await InvalidView();
			}

			try
			{

				var shiftEntity = await _shiftRepository.GetByIDAsync(ShiftId);
				if (shiftEntity == null)
				{
					return NotFound();
				}

                var shiftEntitys = await _shiftRepository.GetAllAsync();
                var startTime = shift.StartTime;
                var endTime = shift.EndTime;
                TimeSpan duration;
                duration = endTime - startTime;
                decimal workingHours = (decimal)duration.TotalHours;

                if (workingHours < 1)
                {
                    return await InvalidView("Cannot create a Shift with the total hour smaller than one hour!");
                }

                if (shift.EndTime < shift.StartTime)
                {
                    return await InvalidView("Cannot create a Shift with the start time greater than end time!");
                }

                foreach (Shift item in shiftEntitys)
                {
                    if (item.StartTime == shift.StartTime && item.EndTime == shift.EndTime && item.ShiftId != ShiftId)
                    {
                        return await InvalidView("This Shift has already exists!");
                    }
                }

                shiftEntity.ShiftName = shift.ShiftName;
				shiftEntity.StartTime = shift.StartTime;
				shiftEntity.EndTime = shift.EndTime;

				await _shiftRepository.UpdateAsync(shiftEntity);

				// Reschedule the job
				await _scheduler.ScheduleCheckAttendanceJob(shiftEntity.ShiftId, shiftEntity.StartTime, shiftEntity.EndTime);
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

				// UnSchedule the job
				await _scheduler.UnScheduleJob(ShiftId);
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
