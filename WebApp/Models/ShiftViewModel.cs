using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using System.ComponentModel.DataAnnotations;
using WebApp.Models;

namespace WebApp.Models
{
	public class ShiftViewModel
	{
		public int ShiftId { get; set; }

		[Required(ErrorMessage = "Shift name is required.")]
		[StringLength(255, ErrorMessage = "Shift name cannot be longer than 255 characters.")]
		[Display(Name = "Shift Name")]
		public string ShiftName { get; set; }

		[Required(ErrorMessage = "Start time is required.")]
		[Display(Name = "Start Time")]
		public TimeOnly StartTime { get; set; }

		[Required(ErrorMessage = "End time is required.")]
		[Display(Name = "End Time")]
		public TimeOnly EndTime { get; set; }

		public ShiftViewModel()
		{
		}

		public ShiftViewModel(Shift shift)
		{
			ShiftId = shift.ShiftId;
			ShiftName = shift.ShiftName;
			StartTime = shift.StartTime;
			EndTime = shift.EndTime;
		}
	}
}
