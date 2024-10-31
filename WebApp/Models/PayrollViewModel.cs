using System.ComponentModel.DataAnnotations;
using Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Models
{
    public class PayrollViewModel
    {
        public long? PayrollId { get; set; }

        [Required(ErrorMessage = "Created By is required.")]
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }

        [Required(ErrorMessage = "Created At is required.")]
        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Employee ID is required.")]
        [Display(Name = "Employee")]
        public int EmpId { get; set; }


        [Required(ErrorMessage = "Month is required.")]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        [Display(Name = "Month")]
        public byte Month { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        [Range(1900, 9999, ErrorMessage = "Year must be a valid year.")]
        [Display(Name = "Year")]
        public short Year { get; set; }

        [Required(ErrorMessage = "Working Hours is required.")]
        [Range(0, 9999.99, ErrorMessage = "Working Hours must be between 0 and 9999.99.")]
        [Display(Name = "Working Hours")]
        public decimal? WorkingHours { get; set; }

        [Required(ErrorMessage = "Salary is required.")]
        [Range(0, 99999999.99, ErrorMessage = "Salary must be between 0 and 99999999.99.")]
        [Display(Name = "Salary")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(typeof(PayrollStatus), ErrorMessage = "Invalid status.")]
        [Display(Name = "Status")]
        public PayrollStatus Status { get; set; }

        [Display(Name = "Payment Date")]
        [DataType(DataType.Date)]
        public DateOnly? PaymentDate { get; set; }
        public string? EmployeeName { get;  set; }

        public IEnumerable<SelectListItem>? EmployeeList { get; set; }
        public User? Employee { get; set; }

        public PayrollViewModel() { }

        public PayrollViewModel(Payroll payroll)
        {
            PayrollId = payroll.PayrollId;
            CreatedBy = payroll.CreatedBy;
            CreatedAt = payroll.CreatedAt;
            EmpId = payroll.EmpId;
            Month = payroll.Month;
            Year = payroll.Year;
            WorkingHours = payroll.WorkingHours;
            Salary = payroll.Salary;
            Status = payroll.Status;
            PaymentDate = payroll.PaymentDate;
            Employee = payroll.Employee;
        }
    }
}
