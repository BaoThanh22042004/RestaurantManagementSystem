using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Models.Entities;
using Repositories;
using Repositories.Interface;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Utilities;

namespace WebApp.Controllers
{
    [Route("Dashboard/Schedule")]
    [Authorize(Policy = "Staff")]
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
            var user = (await _userRepository.GetAllAsync()).Where(u => u.Role != Role.Customer && u.IsActive.Equals(true)).Select(u => new SelectListItem
            {
                Value = u.UserId.ToString(),
                Text = u.FullName,
            }).ToList();
            return user;
        }

        private async Task<IEnumerable<SelectListItem>> GetUserRole()
        {
            var userRole = (await _userRepository.GetAllAsync()).Where(u => u.Role != Role.Customer && u.IsActive.Equals(true)).Select(u => new SelectListItem
            {
                Value = u.UserId.ToString(),
                Text = u.Role.ToString()
            }).ToList();
            return userRole;
        }

        public async Task<IActionResult> Index(string week)
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
                EmployeeOptions = await GetUserList(),
                ScheDate = DateOnly.FromDateTime(DateTime.Now)
            };
            return PartialView("_CreateScheduleModal", schedule);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(ScheduleViewModel scheduleViewModel)
        {
            Boolean today = false;
            async Task<IActionResult> InvalidView(string? error = null)
            {
                if (error != null)
                {
                    TempData["Error"] = error;
                }
                scheduleViewModel.ShiftOptions = await GetShiftList();
                scheduleViewModel.EmployeeOptions = await GetUserList();
                return PartialView("_CreateScheduleModal", scheduleViewModel);
            }

            if (!ModelState.IsValid)
            {
                return await InvalidView();
            }

            try
            {
                if (scheduleViewModel.ScheDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    return await InvalidView("Cannot create a schedule with the date in the past!");
                }
                else if (scheduleViewModel.ScheDate == DateOnly.FromDateTime(DateTime.Now))
                {
                    today = true;
                }

                var shift = await _shiftRepository.GetByIDAsync(scheduleViewModel.ShiftId);

                if (shift == null)
                {
                    return await InvalidView("Shift does not exist!");
                }

                if (shift.EndTime <= TimeOnly.FromDateTime(DateTime.Now).AddMinutes(-30) && today)
                {
                    return await InvalidView("Cannot create a schedule with end time less than thirty minutes of current time!");
                }

                var schedules = await _scheduleRepository.GetAllAsync();
                var scheduleList = schedules.Select(schedule => new ScheduleViewModel(schedule));
                foreach (var schelist in scheduleList.Where(d => d.ScheDate == scheduleViewModel.ScheDate && d.EmpId == scheduleViewModel.EmpId))
                {
                    if (schelist.ShiftId == scheduleViewModel.ShiftId)
                    {
                        return await InvalidView("Shift already exists for this day!");
                    }
                }

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
                return await InvalidView("An error occurred while creating schedule. Please try again later.");
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
            Boolean today = false;
            async Task<IActionResult> InvalidView(string? error = null)
            {
                if (error != null)
                {
                    TempData["Error"] = error;
                }
                schedule.ShiftOptions = await GetShiftList();
                schedule.EmployeeOptions = await GetUserList();
                return PartialView("_EditScheduleModal", schedule);
            }

            if (!ModelState.IsValid)
            {
                return await InvalidView();
            }

            try
            {
                var scheduleEntity = await _scheduleRepository.GetByIDAsync(ScheId);
                if (scheduleEntity == null)
                {
                    return NotFound();
                }
              
                if (scheduleEntity.Attendance?.Status != null) 
                {
                    return await InvalidView("Cannot edit a schedule has already clock in or clock out!");
                }

                if (schedule.ScheDate < DateOnly.FromDateTime(DateTime.Now))
                {
                    return await InvalidView("Cannot edit a schedule with the date in the past!");
                }
                else if (schedule.ScheDate == DateOnly.FromDateTime(DateTime.Now))
                {
                    today = true;
                }

                var shift = await _shiftRepository.GetByIDAsync(schedule.ShiftId);

                if (shift.EndTime <= TimeOnly.FromDateTime(DateTime.Now).AddMinutes(-30) && today) 
                {
                    return await InvalidView("Cannot edit a schedule with end time less than thirty minutes of current time!");
                }

                var schedules = await _scheduleRepository.GetAllAsync();
                var scheduleList = schedules.Where(d => d.ScheDate == schedule.ScheDate && d.EmpId == schedule.EmpId).Select(schedule => new ScheduleViewModel(schedule));

                foreach (var schelist in scheduleList)
                {
                    if (scheduleEntity.ShiftId == schedule.ShiftId && schedule.ScheDate == scheduleEntity.ScheDate)
                    {
                        return Json(new { success = true });
                    }
                    if (schelist.ShiftId == schedule.ShiftId)
                    {
                        return await InvalidView("Shift already exists for this day!");
                    }
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
                schedule.ShiftOptions = await GetShiftList();
                schedule.EmployeeOptions = await GetUserList();
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

            var scheduleViewModel = new ScheduleViewModel(schedule);
            return PartialView("_DeleteScheduleModal", scheduleViewModel);
        }

        [HttpPost]
        [Route("Delete/{ScheId}")]
        public async Task<IActionResult> DeleteConfirmed(int ScheId)
        {
            try
            {
                var scheduleEntity = await _scheduleRepository.GetByIDAsync(ScheId);
                if (scheduleEntity == null)
                {
                    return NotFound();
                }

                if (scheduleEntity.Attendance?.Status == null)
                {
                    await _scheduleRepository.DeleteAsync(ScheId);
                }
                else
                {
                    TempData["Error"] = "Cannot delete the Schedule which has been ClockIn!";
                    return RedirectToAction("Delete", new { id = ScheId });
                }
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
