using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Dish
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DishId { get; set; }
        [StringLength(255)]
        public required string DishName { get; set; }
        public required decimal Price { get; set; }
        public string? Description { get; set; }
        public required bool Visible { get; set; }
        public required int CategoryId { get; set; }

        // Navigation property
        public DishCategory Category { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
