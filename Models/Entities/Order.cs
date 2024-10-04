using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public enum OrderStatus
    {
        Reservation,
        Serving,
        Paid
    }

    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long OrderId { get; set; }

        public int? TableId { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required OrderStatus Status { get; set; }

        public long? ResId { get; set; }

        // Navigation property
        public Reservation Reservation { get; set; } = null!;
        public Table Table { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
