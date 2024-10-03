using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Models.Entities
{
	public class Storage
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ItemId { get; set; }

		[StringLength(255)]
		public required string ItemName { get; set; }

		[StringLength(100)]
		public required string Unit { get; set; }

		[Precision(10, 2)]
		public required decimal Quantity { get; set; }

		// Navigation property
		public ICollection<StorageLog> StorageLogs { get; set; } = new List<StorageLog>();
	}
}
