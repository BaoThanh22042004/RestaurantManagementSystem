using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class UserViewModel
	{
		public int? UserId { get; set; }

		[Required(ErrorMessage = "Username is required.")]
		[StringLength(255, ErrorMessage = "Username must not exceed 255 characters.")]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[StringLength(255, MinimumLength = 8, ErrorMessage = "Password must between 8 and 255 characters.")]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string? Password { get; set; }

		[Required(ErrorMessage = "Full name is required.")]
		[StringLength(255, ErrorMessage = "Full name must not exceed 255 characters.")]
		[Display(Name = "Full Name")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		[StringLength(255, ErrorMessage = "Email must not exceed 255 characters.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Phone number is required.")]
		[StringLength(15, ErrorMessage = "Phone number must not exceed 15 characters.")]
		[Phone(ErrorMessage = "Invalid phone number.")]
		[Display(Name = "Phone Number")]
		public string Phone { get; set; }

		[Required(ErrorMessage = "Role is required.")]
		[EnumDataType(typeof(Role), ErrorMessage = "Invalid role.")]
		public Role Role { get; set; }

		[Range(0, 100000000, ErrorMessage = "Salary must be between 0 and 100000000.")]
		[Precision(10, 2)]
		[Display(Name = "Salary")]
		public decimal? Salary { get; set; }

		[Display(Name = "Is Active")]
		public bool IsActive { get; set; }

		public UserViewModel()
		{
		}

		public UserViewModel(User user)
		{
			UserId = user.UserId;
			Username = user.Username;
			FullName = user.FullName;
			Email = user.Email;
			Phone = user.Phone;
			Role = user.Role;
			Salary = user.Salary;
			IsActive = user.IsActive;
		}
	}
}
