using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Repositories.Interface;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Utilities;

namespace WebApp.Controllers
{
    [Route("Dashboard/Schedule")]
	[Authorize(Roles = $"{nameof(Role.Manager)},{nameof(Role.Waitstaff)}")]
	public class ScheduleController : Controller
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IShiftRepository _shiftRepository;
        private readonly IUserRepository _userRepository;

        public ScheduleController(IScheduleRepository scheduleRepository, IShiftRepository shiftRepository, IUserRepository userRepository)
        {
            _scheduleRepository = scheduleRepository;
            _shiftRepository = shiftRepository;
            _userRepository = userRepository;
        }

        private async Task<IEnumerable<SelectListItem>> GetShiftList() 
        {
            var shift = (await _shiftRepository.GetAllAsync()).Select(s => new SelectListItem
			{
                Value = s.ShiftId.ToString(),
                Text = s.ShiftName
            });
            return shift;
        }

        private async Task<IEnumerable<SelectListItem>> GetUserList()
        {
            var user = (await _userRepository.GetAllAsync()).Where(u => u.Role != Role.Customer).Select(u => new SelectListItem
            {
                Value = u.UserId.ToString(),
                Text =  u.FullName,             
            }).ToList();
            return user;
        }

        private async Task<IEnumerable<SelectListItem>> GetUserRole()
        {
            var userRole = (await _userRepository.GetAllAsync()).Where(u => u.Role != Role.Customer).Select(u => new SelectListItem
            {
                Value = u.UserId.ToString(),
                Text = u.Role.ToString()
            }).ToList();
            return userRole;
        }

        public async Task<IActionResult> Index()
        {
			var allShifts = await GetShiftList();

			var allUsers = await GetUserList();

            var allUserRoleByIds = await GetUserRole();

			var schedules = await _scheduleRepository.GetAllAsync();
            var scheduleList = schedules.Select(schedule => new ScheduleViewModel(schedule)
            {
                ShiftOptions = allShifts,
                EmployeeOptions = allUsers,
                EmployeeRoleOptions = allUserRoleByIds
            });
  
            return View("ScheduleView", scheduleList);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var schedule = new ScheduleViewModel()
            {
                ShiftOptions = await GetShiftList(),
                EmployeeOptions = await GetUserList()
            };
            return PartialView("_CreateScheduleModal", schedule);
        }

		[HttpPost("Create")]
		public async Task<IActionResult> Create(ScheduleViewModel scheduleViewModel)
		{

            if (!ModelState.IsValid)
            {
                scheduleViewModel.ShiftOptions = await GetShiftList();
                scheduleViewModel.EmployeeOptions = await GetUserList();
                return PartialView("_CreateScheduleModal", scheduleViewModel);
            }

			try
			{
				var schedule = new Schedule
				{
					ScheDate = scheduleViewModel.ScheDate,
					EmpId = scheduleViewModel.EmpId,
					ShiftId = scheduleViewModel.ShiftId,
				};
				await _scheduleRepository.InsertAsync(schedule);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while creating schedule. Please try again later.";
				return PartialView("_CreateScheduleModal", scheduleViewModel);
			}

			return Json(new { success = true });
		}

		[HttpGet("Details/{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var schedule = await _scheduleRepository.GetByIDAsync(id);
			if (schedule == null)
			{
				return NotFound();
			}

            var scheduleViewModel = new ScheduleViewModel(schedule) 
            {
                ShiftOptions = await GetShiftList(),
                EmployeeOptions = await GetUserList()
            };
            return PartialView("_DetailsScheduleModal", scheduleViewModel);
        }

		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var schedule = await _scheduleRepository.GetByIDAsync(id);
			if (schedule == null)
			{
				return NotFound();
			}

            var scheduleViewModel = new ScheduleViewModel(schedule);
            scheduleViewModel.ShiftOptions = await GetShiftList();
            scheduleViewModel.EmployeeOptions = await GetUserList();
            return PartialView("_EditScheduleModal", scheduleViewModel);
        }

        [HttpPost]
        [Route("Edit/{ScheId}")]
        public async Task<IActionResult> Edit(ScheduleViewModel schedule, int ScheId)
        {
            if (!ModelState.IsValid)
            {
                schedule.ShiftOptions = await GetShiftList();
                schedule.EmployeeOptions = await GetUserList();
                return PartialView("_EditScheduleModal", schedule);
            }

			try
			{
				var scheduleEntity = await _scheduleRepository.GetByIDAsync(ScheId);
				if (scheduleEntity == null)
				{
					return NotFound();
				}

				scheduleEntity.ScheDate = schedule.ScheDate;
				scheduleEntity.EmpId = schedule.EmpId;
				scheduleEntity.ShiftId = schedule.ShiftId;

				await _scheduleRepository.UpdateAsync(scheduleEntity);
			}
			catch (InvalidOperationException)
			{
				return NotFound();
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while updating schedule. Please try again later.";
				return PartialView("_EditScheduleModal", schedule);
			}

			return Json(new { success = true });
        }

		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var schedule = await _scheduleRepository.GetByIDAsync(id);
			if (schedule == null)
			{
				return NotFound();
			}

            var scheduleViewModel = new ScheduleViewModel(schedule)
            {
                ShiftOptions = await GetShiftList(),
                EmployeeOptions = await GetUserList()
            };
            return PartialView("_DeleteScheduleModal", scheduleViewModel);
        }

		[HttpPost]
		[Route("Delete/{ScheId}")]
		public async Task<IActionResult> DeleteConfirmed(int ScheId)
		{
			try
			{
				await _scheduleRepository.DeleteAsync(ScheId);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while deleting schedule. Please try again later.";
				return RedirectToAction("Delete", new { id = ScheId });
			}

			return Json(new { success = true });
        }
	}
}
