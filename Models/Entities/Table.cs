using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entities
{
	public enum TableStatus
	{
		Available,
		Reserved,
		Occupied,
		Cleaning,
		Unavailable
	}

	public class Table
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int TableId { get; set; }

		[StringLength(50)]
		public required string TableName { get; set; }

		public required short Capacity { get; set; }

		public required TableStatus Status { get; set; }

		public TimeOnly? ResTime { get; set; }

		[Column(TypeName = "text")]
		public string? Notes { get; set; }

		// Navigation property   
		public ICollection<Order> Orders { get; set; } = new List<Order>();
	}
}
