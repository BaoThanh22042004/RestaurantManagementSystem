using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Repositories.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("DashBoard/Payment")]
    [Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Accountant)}")]
    public class PaymentController : Controller
    {
        private readonly IBillRepository _billRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;

        public PaymentController(IBillRepository billRepository, IOrderRepository orderRepository,
                                 IOrderItemRepository orderItemRepository)
        {
            _billRepository = billRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<IActionResult> Index()
        {
            var bills = await _billRepository.GetAllAsync();
            var billList = bills.Select(bill => new PaymentViewModel(bill));
            return View("PaymentView", billList);
        }

        private async Task<IEnumerable<SelectListItem>> GetBillList()
        {
            var bills = (await _billRepository.GetAllAsync()).Select(b => new SelectListItem
            {
                Value = b.BillId.ToString(),
                Text = b.BillId.ToString()
            });
            return bills;
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create(int orderId)
        {
            var orderItems = (await _orderItemRepository.GetAllByOrderIdAsync(orderId))
                .Select(item => new OrderItemViewModel(item)).ToList();
            var payment = new PaymentViewModel
            {
                OrderItems = orderItems,
                OrderId = orderId,
                TotalAmount = await CalculateTotalAmount(orderId)
            };

            return PartialView("_CreatePaymentModal", payment);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(PaymentViewModel paymentViewModel)
        {
            if (!ModelState.IsValid)
            {
                paymentViewModel.TotalAmount = await CalculateTotalAmount(paymentViewModel.OrderId);
                return PartialView("_CreatePaymentModal", paymentViewModel);
            }

            try
            {
                var bill = new Bill
                {
                    OrderId = paymentViewModel.OrderId,
                    CreatedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    CreatedAt = DateTime.Now,
                    TotalAmount = await CalculateTotalAmount(paymentViewModel.OrderId),
                    PaymentMethod = paymentViewModel.PaymentMethod
                };
                await _billRepository.InsertAsync(bill);

                var order = await _orderRepository.GetByIDAsync(paymentViewModel.OrderId);
                order.Status = OrderStatus.Paid;
                await _orderRepository.UpdateAsync(order);
            }

            catch (Exception e)
            {
                TempData["Error"] = "Failed to create payment.";
                paymentViewModel.TotalAmount = await CalculateTotalAmount(paymentViewModel.OrderId);
                return PartialView("_CreatePaymentModal", paymentViewModel);
            }
            return Json(new { success = true });
        }

        private async Task<IEnumerable<Order>> GetOrderList()
        {
            return await _orderRepository.GetAllAsync();
        }

        private async Task<decimal> CalculateTotalAmount(long orderId)
        {
            var order = await _orderRepository.GetByIDAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }
            return order.OrderItems.Sum(item => item.Price * item.Quantity);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            var bill = await _billRepository.GetByIDAsync(id);
            if (bill == null) return NotFound();

            var payment = new PaymentViewModel(bill);
            var order = await _orderRepository.GetByIDAsync(bill.OrderId);

            payment.OrderItems = (await _orderItemRepository.GetAllAsync())
                                    .Where(item => item.OrderId == bill.OrderId)
                                    .Select(item =>
                                    {
                                        var viewModel = new OrderItemViewModel(item);
                                        viewModel.DishName = order?.OrderItems
                                                                .FirstOrDefault(i => i.DishId == item.DishId)?
                                                                .Dish?.DishName ?? "Unknown Dish";
                                        return viewModel;
                                    }).ToList();

            return PartialView("_DetailsPaymentModal", payment);
        }
    }
}