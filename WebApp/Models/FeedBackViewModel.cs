using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class FeedBackViewModel
	{
		[Display(Name = "Feedback ID")]
		public long? FeedbackId { get; set; }

		[Display(Name = "Full Name")]
		[StringLength(255, ErrorMessage = "Full name must be a maximum of 255 characters.")]
		[Required(ErrorMessage = "Full name is required.")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid Email Address.")]
		[StringLength(255, ErrorMessage = "Email must be a maximum of 255 characters.")]
		[Display(Name = "Email Address")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Phone is required.")]
		[Phone(ErrorMessage = "Invalid phone number.")]
		[StringLength(15, ErrorMessage = "Phone number must be a maximum of 15 characters.")]
		[Display(Name = "Phone")]
		public string Phone { get; set; }

		[Display(Name = "Subject")]
		[Required(ErrorMessage = "Subject is required.")]
		[StringLength(255, ErrorMessage = "Subject must be a maximum of 255 characters.")]
		public string Subject { get; set; }

		[Display(Name = "Message")]
		[Required(ErrorMessage = "Message is required.")]
		[DataType(DataType.MultilineText)]
		public string Body { get; set; }

		[EnumDataType(typeof(FeedBackStatus), ErrorMessage = "Invalid feedback status.")]
		[Display(Name = "Status")]
		public FeedBackStatus Status { get; set; }

		[Display(Name = "Created At")]
		[DataType(DataType.DateTime)]
		public DateTime CreateAt { get; set; }

		[Display(Name = "Notes")]
		[DataType(DataType.MultilineText)]
		public string? Note { get; set; }

		public FeedBackViewModel()
		{
		}

		public FeedBackViewModel(FeedBack feedback)
		{
			FeedbackId = feedback.FeedbackId;
			FullName = feedback.FullName;
			Email = feedback.Email;
			Phone = feedback.Phone;
			Subject = feedback.Subject;
			Body = feedback.Body;
			Status = feedback.Status;
			CreateAt = feedback.CreateAt;
			Note = feedback.Note;
		}
	}
}
