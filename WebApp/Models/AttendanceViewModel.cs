using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class AttendanceViewModel
    {
        public long? AttendId { get; set; }

        [Required(ErrorMessage = "Schedule is required.")]
        [Display(Name = "Schedule Date")]
        public long ScheId { get; set; }
        public DateOnly? ScheDate { get; set; }
        public IEnumerable<SelectListItem>? ScheduleOptions { get; set; }

        [Required(ErrorMessage = "Check in is required.")]
        [Display(Name = "Check In")]
        public DateTime CheckIn { get; set; }

        [Display(Name = "Check Out")]
        public DateTime? CheckOut { get; set; }

        [Display(Name = "Working Hour")]
        public decimal? WorkingHours { get; set; }

        [Required(ErrorMessage = "Attendance status is required.")]
        [EnumDataType(typeof(AttendanceStatus), ErrorMessage = "Invalid attendance status.")]
        [Display(Name = "Attendance Status")]
        public AttendanceStatus Status { get; set; }

        public string EmployeeName { get; set; }

		public AttendanceViewModel()
        {
        }

        public AttendanceViewModel(Attendance attendance)
        {
            AttendId = attendance.AttendId;
            ScheId = attendance.ScheId;
            ScheDate = attendance.Schedule?.ScheDate;
            CheckIn = attendance.CheckIn;
            CheckOut = attendance.CheckOut;
            WorkingHours = attendance.WorkingHours;
            Status = attendance.Status;
			EmployeeName = attendance.Schedule.Employee.FullName;

		}
    }
}
