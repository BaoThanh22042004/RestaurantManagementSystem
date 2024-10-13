using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories;
using Repositories.Interface;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("Dashboard/Schedule")]
    [Authorize(Roles = "Manager , Staff")]  
    public class ScheduleController : Controller
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleController(IScheduleRepository scheduleRepository) 
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task<IActionResult> Index()
        {
            var schedules = await _scheduleRepository.GetAllAsync();
            var scheduleList = schedules.Select(schedule => new ScheduleViewModel(schedule));
            return View("ScheduleView", scheduleList);
        }

        [Route("Create")]
        public IActionResult Create()
        {
            return View("CreateScheduleView");
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Create(ScheduleViewModel scheduleViewModel)
        {

            if (!ModelState.IsValid)
            {
                return View("CreateScheduleView", scheduleViewModel);
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
            catch (ArgumentException e)
            {
                TempData["Error"] = e.Message;
                return View("CreateScheduleView", scheduleViewModel);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while creating schedule. Please try again later.";
                return View("CreateScheduleView", scheduleViewModel);
            }

            return RedirectToAction("Index");
        }

        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var schedule = await _scheduleRepository.GetByIDAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            var scheduleViewModel = new ScheduleViewModel(schedule);
            return View("DetailsScheduleView", scheduleViewModel);
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
            return View("EditScheduleView", scheduleViewModel);
        }

        [HttpPost]
        [Route("Edit/{ScheId}")]
        public async Task<IActionResult> Edit(ScheduleViewModel schedule, int ScheId)
        {
            if (!ModelState.IsValid)
            {
                return View("EditScheduleView", schedule);
            }

            try
            {
                var scheduleEntity = await _scheduleRepository.GetByIDAsync(ScheId);
                if (scheduleEntity == null)
                {
                    return NotFound();
                }

                scheduleEntity.ScheId = schedule.ScheId;
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
                return View("EditScheduleView", schedule);
            }

            return RedirectToAction("Index");
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
            return View("DeleteScheduleView", scheduleViewModel);
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

            return RedirectToAction("Index");
        }
    }
}
