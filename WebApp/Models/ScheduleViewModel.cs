using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class ScheduleViewModel
    {
        public long ScheId { get; set; }

        [Required(ErrorMessage = "Schedule date is required.")]
        [Display(Name = "Shedule Date")]
        public DateOnly ScheDate { get; set; }


        [Required(ErrorMessage = "Employee is required.")]
        [Display(Name = "Employee")]
        public int EmpId { get; set; }
        public IEnumerable<SelectListItem>? EmployeeOptions { get; set; }
        public string? EmployeeName { get; set; }


        [Required(ErrorMessage = "Shift is required.")]
        [Display(Name = "Shift")]
        public int ShiftId { get; set; }
        public string? ShiftName { get; set; }
		public IEnumerable<SelectListItem>? ShiftOptions { get; set; }

		public ScheduleViewModel() 
        { 
        }

        public ScheduleViewModel(Schedule schedule) 
        {
            ScheId = schedule.ScheId; 
            ScheDate = schedule.ScheDate; 
            EmpId = schedule.EmpId; 
            ShiftId = schedule.ShiftId; 
            ShiftName = schedule.Shift?.ShiftName;
            EmployeeName = schedule.Employee?.FullName;
        }
    }
}
