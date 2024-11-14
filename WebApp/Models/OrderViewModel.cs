using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class OrderViewModel
    {
        public long OrderId { get; set; }

        [Display(Name = "Table Id")]
        public int? TableId { get; set; }

        [Required(ErrorMessage = "Created At date is required.")]
        [Display(Name = "Order Created At")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Order Status is required.")]
        [Display(Name = "Order Status")]
        public OrderStatus Status { get; set; }

        [Display(Name = "Reservation ID")]
        public long? ResId { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

        public SelectList? TableOptions { get; set; }

        public Reservation? Reservation { get; set; }

		public Table? Table { get; set; }

		public OrderViewModel()
        {

        }

        public OrderViewModel(Order order)
        {
            OrderId = order.OrderId;
            TableId = order.TableId;
            ResId = order.ResId;
            CreatedAt = order.CreatedAt;
            Status = order.Status;
            OrderItems = order.OrderItems.Select(item => new OrderItemViewModel(item)).ToList();
			Reservation = order.Reservation;
			Table = order.Table;
		}
    }
}
