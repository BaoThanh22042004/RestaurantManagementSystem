using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class ScheduleViewModel
    {
        public long ScheId { get; set; }

        [Required(ErrorMessage = "Schedule date is required.")]
        [Display(Name = "Schedule Date")]
        public DateOnly ScheDate { get; set; }

        [Required(ErrorMessage = "Employee is required.")]
        [Display(Name = "Employee")]
        public int EmpId { get; set; }

        [Required(ErrorMessage = "Shift is required.")]
        [Display(Name = "Shift")]
        public int ShiftId { get; set; }

        public ScheduleViewModel() 
        { 
        }

        public ScheduleViewModel(Schedule schedule) 
        {
            ScheId = schedule.ScheId; 
            ScheDate = schedule.ScheDate; 
            EmpId = schedule.EmpId; 
            ShiftId = schedule.ShiftId; 
        }
    }
}
