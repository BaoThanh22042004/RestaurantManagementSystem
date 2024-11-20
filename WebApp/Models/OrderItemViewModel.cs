using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class OrderItemViewModel
    {
        public long OrItemId { get; set; }

        [Required(ErrorMessage = "Order ID is required.")]
        public long OrderId { get; set; }

        [Required(ErrorMessage = "Dish ID is required.")]
        [Display(Name = "Dish ID")]
        public int DishId { get; set; }

        [Required(ErrorMessage = "Created by information is required.")]
        [Display(Name = "Created By (User ID)")]
        public int CreatedBy { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, short.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        
        [Display(Name = "Quantity")]
        public short Quantity { get; set; }

        [Display(Name = "Price (per dish)")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Order item status is required.")]
        [Display(Name = "Order Item Status")]
        public OrderItemStatus Status { get; set; }

        [Display(Name = "Notes")]
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
        public Dish? Dish { get; set; }
        public User? Creator { get; set; }

        public IEnumerable<SelectListItem>? StatusOptions { get; set; }

        public OrderItemViewModel() 
        { 

        }

        public OrderItemViewModel(OrderItem orderItem)
        {
            OrItemId = orderItem.OrItemId;
            OrderId = orderItem.OrderId;
            DishId = orderItem.DishId;
            CreatedBy = orderItem.CreatedBy;
            Quantity = orderItem.Quantity;
            Price = orderItem.Price;
            Status = orderItem.Status;
            Notes = orderItem.Notes;
            Creator = orderItem.Creator;
            Dish = orderItem.Dish;
        }
    }
}
