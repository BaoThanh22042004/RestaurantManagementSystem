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

namespace WebApp.Controllers
{
    [Route("Dashboard/Payroll")]
    [Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Waitstaff)}")]
    public class PayrollController : Controller
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly IUserRepository _userRepository;


        public PayrollController(IPayrollRepository payrollRepository, IUserRepository userRepository)
        {
            _payrollRepository = payrollRepository;
            _userRepository = userRepository;

        }

        public async Task<IActionResult> Index()
        {
            var payrolls = await _payrollRepository.GetAllAsync();
            var payrollList = payrolls.Select(payroll => new PayrollViewModel(payroll));
            return View("PayrollView", payrollList);
        }
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var employees = await _userRepository.GetAllAsync();

            var payrollViewModel = new PayrollViewModel
            {
                EmployeeList = employees.Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.FullName
                }).ToList()
            };

            return PartialView("_CreatePayrollModal", payrollViewModel);
        }


        [HttpPost("Create")]
        public async Task<IActionResult> Create(PayrollViewModel payrollViewModel)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CreatePayrollModal", payrollViewModel);
            }

            try
            {
                
                var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdString, out int userId))
                {
                    
                    ModelState.AddModelError(string.Empty, "Invalid user ID.");
                    return PartialView("_CreatePayrollModal", payrollViewModel);
                }

                var user = await _userRepository.GetByIDAsync(userId);
                var payroll = new Payroll
                {
                    CreatedBy = userId, 
                    CreatedAt = DateTime.Now,
                    EmpId = payrollViewModel.EmpId,
                    Month = payrollViewModel.Month,
                    Year = payrollViewModel.Year,
                    WorkingHours = (decimal)payrollViewModel.WorkingHours,
                    Salary = payrollViewModel.Salary,
                    Status = PayrollStatus.UnPaid,
                    PaymentDate = payrollViewModel.PaymentDate
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

            var employee = await _userRepository.GetByIDAsync(payroll.EmpId); 
            var payrollViewModel = new PayrollViewModel(payroll)
            {
                EmployeeName = employee?.FullName
            };
            return PartialView("_EditPayrollModal", payrollViewModel);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(PayrollViewModel payrollViewModel)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_EditPayrollModal", payrollViewModel);
            }

            try
            {
                var id = payrollViewModel.PayrollId;
                var payrollEntity = await _payrollRepository.GetByIDAsync(id: (long)id);
                if (payrollEntity == null)
                {
                    return NotFound();
                }

                payrollEntity.EmpId = payrollViewModel.EmpId;
                payrollEntity.Month = payrollViewModel.Month;
                payrollEntity.Year = payrollViewModel.Year;
                payrollEntity.WorkingHours = (decimal)payrollViewModel.WorkingHours;
                payrollEntity.Salary = payrollViewModel.Salary;
                payrollEntity.Status = payrollViewModel.Status;
                payrollViewModel.PaymentDate = payrollViewModel.PaymentDate;

                await _payrollRepository.UpdateAsync(payrollEntity);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while updating the payroll. Please try again later.";
                return PartialView("_EditPayrollModal", payrollViewModel);
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
                return RedirectToAction("Delete", new { payrollId });
            }

            return Json(new { success = true });
        }
    }
}
