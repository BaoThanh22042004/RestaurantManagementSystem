using System.ComponentModel.DataAnnotations;
using Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Models
{
    public class PayrollViewModel
    {
        [Display(Name = "Payroll ID")]
        public long? PayrollId { get; set; }

        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Employee is required.")]
        [Display(Name = "Employee")]
        public int EmpId { get; set; }

        [Display(Name = "Month/Year")]
        [Required(ErrorMessage = "Month/Year is required.")]
		public DateOnly MonthAndYear { get; set; }

        [Display(Name = "Working Hours")]
        public decimal? WorkingHours { get; set; }

        [Display(Name = "Hourly Salary")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(typeof(PayrollStatus), ErrorMessage = "Invalid status.")]
        [Display(Name = "Status")]
        public PayrollStatus Status { get; set; }

        [Display(Name = "Payment Date")]
        [DataType(DataType.Date)]
        public DateOnly? PaymentDate { get; set; }


        public SelectList? EmployeeList { get; set; }
        public User? Employee { get; set; }
        public User? Creator { get; set; }

        public PayrollViewModel() { }

        public PayrollViewModel(Payroll payroll)
        {
            PayrollId = payroll.PayrollId;
            CreatedBy = payroll.CreatedBy;
            CreatedAt = payroll.CreatedAt;
            EmpId = payroll.EmpId;
            MonthAndYear = new DateOnly(payroll.Year, payroll.Month, 1);
            WorkingHours = payroll.WorkingHours;
            Salary = payroll.Salary;
            Status = payroll.Status;
            PaymentDate = payroll.PaymentDate;
            Employee = payroll.Employee;
			Creator = payroll.Creator;
		}
    }
}
