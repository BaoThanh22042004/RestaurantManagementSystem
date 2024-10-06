using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class ForgotPasswordViewModel
	{
		[Required(ErrorMessage = "Username is required")]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		[Display(Name = "Email")]
		public string Email { get; set; }

	}
}
