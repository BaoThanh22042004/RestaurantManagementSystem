using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "Username is required.")]
		[StringLength(255, MinimumLength = 1, ErrorMessage = "Username must be between 1 and 255 characters.")]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[StringLength(255, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Confirm password is required.")]
		[Compare("Password", ErrorMessage = "Passwords do not match.")]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm Password")]
		public string ConfirmPassword { get; set; }

		[Required(ErrorMessage = "Full name is required.")]
		[StringLength(255, MinimumLength = 1, ErrorMessage = "Full name must be between 1 and 255 characters.")]
		[Display(Name = "Full Name")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		[StringLength(255, MinimumLength = 1, ErrorMessage = "Email must be between 1 and 255 characters.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Phone number is required.")]
		[StringLength(15, MinimumLength = 1, ErrorMessage = "Phone number must be between 1 and 15 characters.")]
		[Phone(ErrorMessage = "Invalid phone number.")]
		[Display(Name = "Phone Number")]
		public string Phone { get; set; }

		[Display(Name = "Role")]
		public Role Role { get; set; }

		[Display(Name = "Is Active")]
		public bool IsActive { get; set; }
	}
}
