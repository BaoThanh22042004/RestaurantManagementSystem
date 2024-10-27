using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Repositories;
using Repositories.Interface;
using System;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("Dashboard/Attendance")]
    [Authorize(Roles = $"{nameof(Role.Manager)},{nameof(Role.Waitstaff)}")]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public AttendanceController(IAttendanceRepository attendanceRepository, IScheduleRepository scheduleRepository)
        {
            _attendanceRepository = attendanceRepository;
            _scheduleRepository = scheduleRepository;
        }

        private async Task<IEnumerable<SelectListItem>> GetScheList()
        {
            var schedule = (await _scheduleRepository.GetAllAsync()).Select(s => new SelectListItem
            {
                Value = s.ScheId.ToString(),
                Text = s.ScheDate.ToString()
            });
            return schedule;
        }

        public async Task<IActionResult> Index()
        {
            var allSchedules = await GetScheList();

            var attendances = await _attendanceRepository.GetAllAsync();
            var attendanceList = attendances.Select(attendance => new AttendanceViewModel(attendance)
            {
                ScheduleOptions = allSchedules
            });

            return View("AttendanceView", attendanceList);
        }

        [HttpGet("ConfirmClockIn")]
        public async Task<IActionResult> ConfirmClockIn(long id) 
        {
            var scheduleEntity = await _scheduleRepository.GetByIDAsync(id);
            var scheduleViewModel = new ScheduleViewModel(scheduleEntity);

            return PartialView("_ConfirmClockInModal", scheduleViewModel);
        }

        [HttpGet("ConfirmClockOut")]
        public async Task<IActionResult> ConfirmClockOut(long id)
        {
            var scheduleEntity = await _scheduleRepository.GetByIDAsync(id);
            var scheduleViewModel = new ScheduleViewModel(scheduleEntity);

            return PartialView("_ConfirmClockOutModal", scheduleViewModel);
        }

        [HttpPost("RecordIn")]
        public async Task<IActionResult> RecordIn(long ScheId)
        {
            try
            {
                var attendance = new Attendance
                {
                    ScheId = ScheId,
                    CheckIn = DateTime.Now,
                    Status = AttendanceStatus.ClockIn,
                };

                await _attendanceRepository.InsertAsync(attendance);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while creating attendance. Please try again later.";
                return PartialView("_ConfirmClockInModal");
			}
            return Json(new { success = true });
		}

        [HttpPost("RecordOut")]
        public async Task<IActionResult> RecordOut(long ScheId)
        {
            try
            {
                var scheduleEntity = await _scheduleRepository.GetByIDAsync(ScheId);
                if (scheduleEntity == null)
                {
                    return NotFound();
                }

                var attendanceEntity = scheduleEntity.Attendance;
                TimeSpan duration;
                var startTime = attendanceEntity.CheckIn;
                var endTime = DateTime.Now;
                attendanceEntity.CheckOut = endTime;
               
                duration = endTime - startTime;

                decimal workingHours = (decimal)duration.TotalHours;

                attendanceEntity.WorkingHours = workingHours;
                attendanceEntity.Status = AttendanceStatus.ClockOut;

                await _attendanceRepository.UpdateAsync(attendanceEntity);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while updating schedule. Please try again later.";
                return PartialView("_ConfirmClockOutModal");
			}
            return Json(new { success = true });
        }
    }
}
