using System.ComponentModel.DataAnnotations;
using Models.Entities;  // Assuming that the Table entity and TableStatus enum are part of Models.Entities namespace

namespace WebApp.Models
{
	public class TableViewModel
	{
		[Display(Name = "Table ID")]
		public int? TableId { get; set; }

		[Required(ErrorMessage = "Table name is required.")]
		[StringLength(50, MinimumLength = 1, ErrorMessage = "Table name must be between 1 and 50 characters.")]
		[Display(Name = "Table Name")]
		public string TableName { get; set; }

		[Required(ErrorMessage = "Capacity is required.")]
		[Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100.")]
		[Display(Name = "Capacity")]
		public short Capacity { get; set; }

		[Required(ErrorMessage = "Status is required.")]
		[EnumDataType(typeof(TableStatus), ErrorMessage = "Invalid status.")]
		[Display(Name = "Status")]
		public TableStatus Status { get; set; }

		// Sử dụng TimeSpan? để phù hợp với kiểu dữ liệu trong Entity
		[Display(Name = "Reservation Time")]
		public TimeOnly? ResTime { get; set; }

		[Display(Name = "Notes")]
		[DataType(DataType.MultilineText)]
		public string? Notes { get; set; }


		public Order? Order { get; set; }

		// Default constructor
		public TableViewModel()
		{
		}

		// Constructor to initialize from a Table entity
		public TableViewModel(Table table)
		{
			TableId = table.TableId;
			TableName = table.TableName;
			Capacity = table.Capacity;
			Status = table.Status;
			ResTime = table.ResTime; 
			Notes = table.Notes;
			Order = table.Orders.FirstOrDefault();
		}
	}
}
