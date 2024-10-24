using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using WebApp.Models;

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

        // GET: Payroll/Index
        public async Task<IActionResult> Index()
        {
            var payrolls = await _payrollRepository.GetAllAsync();
            var payrollList = payrolls.Select(payroll => new PayrollViewModel(payroll));
            return View("PayrollView", payrollList);
        }

        // GET: Payroll/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View("CreatePayrollView");
        }

        // POST: Payroll/Create
        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Create(PayrollViewModel payrollViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("CreatePayrollView", payrollViewModel);
            }

            try
            {
                var payroll = new Payroll
                {
                    CreatedBy = payrollViewModel.CreatedBy ?? throw new Exception("CreatedBy is required"),
                    CreatedAt = payrollViewModel.CreatedAt ?? DateTime.Now,
                    EmpId = payrollViewModel.EmpId ?? throw new Exception("Employee ID is required"),
                    Month = payrollViewModel.Month ?? throw new Exception("Month is required"),
                    Year = payrollViewModel.Year ?? throw new Exception("Year is required"),
                    WorkingHours = payrollViewModel.WorkingHours ?? throw new Exception("Working Hours is required"),
                    Salary = payrollViewModel.Salary ?? throw new Exception("Salary is required"),
                    Status = payrollViewModel.Status ?? throw new Exception("Status is required"),
                    PaymentDate = payrollViewModel.PaymentDate // No conversion needed
                };

                await _payrollRepository.InsertAsync(payroll);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while creating the payroll: {ex.Message}.";
                return View("CreatePayrollView", payrollViewModel);
            }

            return RedirectToAction("Index");
        }

        // GET: Payroll/Details/{id}
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            var payroll = await _payrollRepository.GetByIDAsync(id);
            if (payroll == null)
            {
                return NotFound();
            }

            var payrollViewModel = new PayrollViewModel(payroll);
            return View("DetailsPayrollView", payrollViewModel);
        }

        // GET: Payroll/Edit/{id}
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            var payroll = await _payrollRepository.GetByIDAsync(id);
            if (payroll == null)
            {
                return NotFound();
            }

            var payrollViewModel = new PayrollViewModel(payroll);
            return View("EditPayrollView", payrollViewModel);
        }

        // POST: Payroll/Edit/{id}
        [Route("Edit/{id}")]
        [HttpPost]
        public async Task<IActionResult> Edit(PayrollViewModel payrollViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("EditPayrollView", payrollViewModel);
            }

            try
            {
                var payrollEntity = await _payrollRepository.GetByIDAsync(payrollViewModel.PayrollId ?? throw new KeyNotFoundException());
                if (payrollEntity == null)
                {
                    throw new KeyNotFoundException("Payroll record not found.");
                }

                payrollEntity.CreatedBy = payrollViewModel.CreatedBy ?? payrollEntity.CreatedBy;
                payrollEntity.CreatedAt = payrollViewModel.CreatedAt ?? payrollEntity.CreatedAt;
                payrollEntity.EmpId = payrollViewModel.EmpId ?? payrollEntity.EmpId;
                payrollEntity.Month = payrollViewModel.Month ?? payrollEntity.Month;
                payrollEntity.Year = payrollViewModel.Year ?? payrollEntity.Year;
                payrollEntity.WorkingHours = payrollViewModel.WorkingHours ?? payrollEntity.WorkingHours;
                payrollEntity.Salary = payrollViewModel.Salary ?? payrollEntity.Salary;
                payrollEntity.Status = payrollViewModel.Status ?? payrollEntity.Status;
                payrollEntity.PaymentDate = payrollViewModel.PaymentDate; // No conversion needed

                await _payrollRepository.UpdateAsync(payrollEntity);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while updating the payroll: {ex.Message}.";
                return View("EditPayrollView", payrollViewModel);
            }

            return RedirectToAction("Index");
        }

        // GET: Payroll/Delete/{id}
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var payroll = await _payrollRepository.GetByIDAsync(id);
            if (payroll == null)
            {
                return NotFound();
            }

            var payrollViewModel = new PayrollViewModel(payroll);
            return View("DeletePayrollView", payrollViewModel);
        }

        // POST: Payroll/Delete/{PayrollId}
        [HttpPost]
        [Route("Delete/{PayrollId}")]
        public async Task<IActionResult> DeleteConfirmed(long PayrollId)
        {
            try
            {
                await _payrollRepository.DeleteAsync(PayrollId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while deleting the payroll: {ex.Message}.";
                return RedirectToAction("Delete", new { PayrollId });
            }

            return RedirectToAction("Index");
        }
    }
}
