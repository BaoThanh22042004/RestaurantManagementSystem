using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using WebApp.Models;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace WebApp.Controllers
{
    [Route("Dashboard/Payroll")]
    [Authorize(Roles = $"{nameof(Role.Manager)}")]
    public class PayrollController : Controller
    {
        private readonly IPayrollRepository _payrollRepository;

        public PayrollController(IPayrollRepository payrollRepository)
        {
            _payrollRepository = payrollRepository;
        }

        // Index view to list payrolls
        public async Task<IActionResult> Index()
        {
            var payrolls = await _payrollRepository.GetAllAsync();
            var payrollList = payrolls.Select(p => new PayrollViewModel(p));
            return View("PayrollView", payrollList);
        }

      
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return PartialView("_CreatePayrollModal");
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
                var payroll = new Payroll
                {
                    CreatedBy = (int)payrollViewModel.CreatedBy,
                    CreatedAt = DateTime.Now,
                    EmpId = (int)payrollViewModel.EmpId,
                    Month = (byte)payrollViewModel.Month,
                    Year = (short)payrollViewModel.Year,
                    WorkingHours = (decimal)payrollViewModel.WorkingHours,
                    Salary = (decimal)payrollViewModel.Salary,
                    Status = (PayrollStatus)payrollViewModel.Status,
                    PaymentDate = payrollViewModel.PaymentDate
                };

                await _payrollRepository.InsertAsync(payroll);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while creating the payroll entry. Please try again later.";
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

            var payrollViewModel = new PayrollViewModel(payroll);
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
            if (!ModelState.IsValid)
            {
                return PartialView("_EditPayrollModal", payrollViewModel);
            }

            try
            {
                if (payrollViewModel.PayrollId == null)
                {
                    throw new KeyNotFoundException();
                }

                var payroll = await _payrollRepository.GetByIDAsync(payrollViewModel.PayrollId.Value);
                if (payroll == null)
                {
                    throw new KeyNotFoundException();
                }

                payroll.Status = payrollViewModel.Status;
                payroll.PaymentDate = payrollViewModel.PaymentDate;

                await _payrollRepository.UpdateAsync(payroll);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while updating the payroll entry. Please try again later.";
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
                TempData["Error"] = "An error occurred while deleting the payroll entry. Please try again later.";
                return RedirectToAction("Delete", new { id = payrollId });
            }

            return Json(new { success = true });
        }
    }
}
