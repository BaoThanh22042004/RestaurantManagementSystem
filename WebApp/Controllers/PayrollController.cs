using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface; 
using WebApp.Models; 
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace WebApp.Controllers
{
    [Route("Dashboard/Payroll")]
    [Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Accountant)}")]
    public class PayrollController : Controller
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        public PayrollController(IPayrollRepository payrollRepository, IUserRepository userRepository, IAttendanceRepository attendanceRepository)
        {
            _payrollRepository = payrollRepository;
            _userRepository = userRepository;
            _attendanceRepository = attendanceRepository;

		}

        public async Task<IActionResult> Index()
        {
            var payrolls = await _payrollRepository.GetAllAsync();
            var payrollList = payrolls.Select(payroll => new PayrollViewModel(payroll));
            return View("PayrollView", payrollList);
        }

        private async Task<SelectList> GetEmployeeList()
		{
			var employees = (await _userRepository.GetAllAsync()).Where(e => e.Role != Role.Customer);
			return new SelectList(employees, "UserId", "FullName");
		}

		[HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var payrollViewModel = new PayrollViewModel
            {
                EmployeeList = await GetEmployeeList()
            };

            return PartialView("_CreatePayrollModal", payrollViewModel);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(PayrollViewModel payrollViewModel)
        {
			async Task<IActionResult> Invalid()
            {
				payrollViewModel.EmployeeList = await GetEmployeeList();
				return PartialView("_CreatePayrollModal", payrollViewModel);
			}

			bool AreMonthsAndYearsEqual(DateOnly date1, DateOnly date2)
			{
				return date1.Year == date2.Year && date1.Month == date2.Month;
			}


			bool isValid = true;
			if (!ModelState.IsValid) isValid = false;
            if (payrollViewModel.EmpId == 0)
            {
				ModelState.AddModelError("EmpId", "Employee is required.");
                isValid = false;
			}
            if (!isValid) return await Invalid();

			try
            {
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdString, out int userId))
                {
					payrollViewModel.EmployeeList = await GetEmployeeList();
					TempData["Error"] = "Failed to get user information. Please log out and log in again.";
					return PartialView("_CreatePayrollModal", payrollViewModel);
                }

                var creator = await _userRepository.GetByIDAsync(userId);
                var workingHours = (await _attendanceRepository.GetAllAsync())
                    .Where(a => a.Schedule.EmpId == payrollViewModel.EmpId)
                    .Where(a => AreMonthsAndYearsEqual(a.Schedule.ScheDate, payrollViewModel.MonthAndYear))
                    .Sum(a => a.WorkingHours);
                var employee = await _userRepository.GetByIDAsync(payrollViewModel.EmpId);

                var payroll = new Payroll
				{
					EmpId = payrollViewModel.EmpId,
					Month = (byte)payrollViewModel.MonthAndYear.Month,
					Year = (short)payrollViewModel.MonthAndYear.Year,
					WorkingHours = workingHours ?? 0,
					Salary = employee.Salary.Value,
					Status = PayrollStatus.UnPaid,
					CreatedBy = creator.UserId,
					CreatedAt = DateTime.Now,
                    
				};

				await _payrollRepository.InsertAsync(payroll);
			}
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while creating the payroll. Please try again later.";
                return PartialView("_CreatePayrollModal", payrollViewModel);
            }

            return Json(new { success = true });
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            var payroll = await _payrollRepository.GetByIDAsync(id);
            if (payroll == null)
            {
                return NotFound();
            }

            var payrollViewModel  = new PayrollViewModel(payroll);
            return PartialView("_DetailsPayrollModal", payrollViewModel);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            var payroll = await _payrollRepository.GetByIDAsync(id);
            if (payroll == null)
            {
                return NotFound();
            }

            var payrollViewModel = new PayrollViewModel(payroll);
            return PartialView("_EditPayrollModal", payrollViewModel);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(PayrollViewModel payrollViewModel)
        {
			var payrollEntity = await _payrollRepository.GetByIDAsync(payrollViewModel.PayrollId.Value);
			if (payrollEntity == null)
			{
				return NotFound();
			}

            var payrollVM = new PayrollViewModel(payrollEntity);

			if (!ModelState.IsValid)
            {
                return PartialView("_EditPayrollModal", payrollVM);
            }

            if (payrollViewModel.Status == PayrollStatus.Paid && payrollViewModel.PaymentDate == null)
            {
                ModelState.AddModelError("PaymentDate", "Payment date is required.");
				return PartialView("_EditPayrollModal", payrollVM);
			}

            try
            {
                payrollEntity.Status = payrollViewModel.Status;
				payrollEntity.PaymentDate = payrollViewModel.PaymentDate;

                await _payrollRepository.UpdateAsync(payrollEntity);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while updating the payroll. Please try again later.";
                return PartialView("_EditPayrollModal", payrollVM);
            }

            return Json(new { success = true });
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var payroll = await _payrollRepository.GetByIDAsync(id);
            if (payroll == null)
            {
                return NotFound();
            }

            var payrollViewModel = new PayrollViewModel(payroll);
            return PartialView("_DeletePayrollModal", payrollViewModel);
        }

        [HttpPost("Delete/{payrollId}")]
        public async Task<IActionResult> DeleteConfirmed(long payrollId)
        {
            try
            {
                await _payrollRepository.DeleteAsync(payrollId);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the payroll. Please try again later.";
                return RedirectToAction("Delete", new { id = payrollId });
            }

            return Json(new { success = true });
        }
    }
}
