using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public enum FeedBackStatus
	{
		Pending,
		Reviewed,
		Resolved,
		Dismissed,
	}
	public class FeedBack
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long FeedbackId { get; set; }

		[StringLength(255)]
		public required string FullName { get; set; }

		[StringLength(255)]
		public required string Email { get; set; }
		[StringLength(15)]
		public required string Phone { get; set; }

		[StringLength(255)]
		public required string Subject { get; set; }

		[DataType(DataType.MultilineText)]
		public required string Body { get; set; }

		public required DateTime CreateAt { get; set; }

		public required FeedBackStatus Status { get; set; }

		[DataType(DataType.MultilineText)]
		public string? Note { get; set; }
	}
}

