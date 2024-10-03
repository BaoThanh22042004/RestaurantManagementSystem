using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public enum Action
    {
        Add,
        Remove,
        Update
    }

    public class StorageLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LogId { get; set; }

        public required int CreatedBy { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required int ItemId { get; set; }

        [Precision(10, 2)]
        public required decimal ChangeQuantity { get; set; }

        [Precision(10, 2)]
        public required decimal RemainQuantity { get; set; }

        public required Action Action { get; set; }

        [Precision(10, 2)]
        public decimal? Cost { get; set; }

        public string? Description { get; set; }

        public Storage Storage { get; set; } = null!;
        public User User { get; set; } = null!;

    }
}
