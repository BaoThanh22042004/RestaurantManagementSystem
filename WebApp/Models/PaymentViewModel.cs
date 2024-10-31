using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class PaymentViewModel
    {
        [Display(Name = "Bill ID")]
        public long BillId { get; set; }

        [Display(Name = "Order ID")]
        public long OrderId { get; set; }

        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0, 100000000, ErrorMessage = "Total amount must be between 0 and 100000000.")]
        [Precision(18, 2)]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [EnumDataType(typeof(PaymentMethod), ErrorMessage = "Invalid payment method.")]
        [Display(Name = "Payment Method")]
        public PaymentMethod? PaymentMethod { get; set; }

        [Display(Name = "Payment Time")]
        [DataType(DataType.DateTime)]
        public DateTime? PaymentTime { get; set; }

        public IEnumerable<Order>? Orders { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

        public List<TableViewModel> Tables { get; set; } = new List<TableViewModel>();

        public IEnumerable<SelectListItem>? TableOptions { get; set; }
        public PaymentViewModel()
        {
        }

        public PaymentViewModel(Bill bill)
        {
            BillId = bill.BillId;
            OrderId = bill.OrderId;
            CreatedBy = bill.CreatedBy;
            CreatedAt = bill.CreatedAt;//
            TotalAmount = bill.TotalAmount;//
            PaymentMethod = bill.PaymentMethod;//
            PaymentTime = bill.PaymentTime;//
        }
    }
}
