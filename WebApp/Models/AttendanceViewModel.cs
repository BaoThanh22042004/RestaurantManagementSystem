using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class AttendanceViewModel
    {

        public long? AttendId { get; set; }

        [Required(ErrorMessage = "Schedule is required.")]
        [Display(Name = "Schedule")]
        public long ScheId { get; set; }

        [Required(ErrorMessage = "Check in is required.")]
        [Display(Name = "Check In")]
        public TimeOnly CheckIn { get; set; }

        [Display(Name = "Check Out")]
        public TimeOnly? CheckOut { get; set; }

        [Display(Name = "Working Hour")]
        public decimal? WorkingHours { get; set; }

        [Required(ErrorMessage = "Attendance status is required.")]
        [EnumDataType(typeof(AttendanceStatus), ErrorMessage = "Invalid attendance status.")]
        [Display(Name = "Attendance Status")]
        public AttendanceStatus Status { get; set; }

        public AttendanceViewModel()
        {
        }

        public AttendanceViewModel(Attendance attendance)
        {
            AttendId = attendance.AttendId;
            ScheId = attendance.ScheId;
            CheckIn = attendance.CheckIn;
            CheckOut = attendance.CheckOut;
            WorkingHours = attendance.WorkingHours;
            Status = attendance.Status;
        }
    }
}
