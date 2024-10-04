using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public enum Action
	{
		Import,
		Export,
		Cancel
	}

	public class StorageLog
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long LogId { get; set; }

		public required int CreatedBy { get; set; }

		public required DateTime CreatedAt { get; set; } = DateTime.Now;

		public required int ItemId { get; set; }

		[Precision(10, 2)]
		public required decimal ChangeQuantity { get; set; }

		[Precision(10, 2)]
		public required decimal RemainQuantity { get; set; }

		public required Action Action { get; set; }

		[Precision(10, 2)]
		public decimal? Cost { get; set; }

		[Column(TypeName = "text")]
		public string? Description { get; set; }

		// Navigation properties
		public Storage StorageItem { get; set; } = null!;

		public User Creator { get; set; } = null!;
	}
}
