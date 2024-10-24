using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int? CustomerId { get; set; }

        [StringLength(255)]
        public string Email { get; set; }
        [StringLength(15)]
        public string Phone { get; set; }
        public string? Subject { get; set; }

        public string? Body { get; set; }
        public DateTime? CreateAt { get; set; }
        public required FeedBackStatus Status { get; set; }

        public User? Customer { get; set; } 
    }
}

