using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
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
    [Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Accountant)}, {nameof(Role.Waitstaff)}")]
    public class PaymentController : Controller
    {
        private readonly IBillRepository _billRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ITableRepository _tableRepository;

        public PaymentController(IBillRepository billRepository, IOrderRepository orderRepository,
                                 IOrderItemRepository orderItemRepository, ITableRepository tableRepository)
        {
            _billRepository = billRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _tableRepository = tableRepository;
        }

        public async Task<IActionResult> Index()
        {
            var bills = await _billRepository.GetAllAsync();
            var billList = bills.Select(bill => new PaymentViewModel(bill));
            return View("PaymentView", billList);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create(int orderId)
        {
            var orderItems = (await _orderItemRepository.GetAllByOrderIdAsync(orderId))?
                .Select(item => new OrderItemViewModel(item)).ToList();
            var payment = new PaymentViewModel
            {
                OrderItems = orderItems ?? new(),
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
                    PaymentMethod = paymentViewModel.PaymentMethod,
                    PaymentTime = DateTime.Now
                };
                await _billRepository.InsertAsync(bill);

                var order = await _orderRepository.GetByIDAsync(paymentViewModel.OrderId);
                if (order == null)
                {
                    throw new KeyNotFoundException("Order not found.");
                }
                order.Status = OrderStatus.Paid;
                await _orderRepository.UpdateAsync(order);

                if (order.TableId.HasValue)
                {
                    var table = await _tableRepository.GetByIDAsync(order.TableId.Value);
                    if (table != null)
                    {
                        table.Status = TableStatus.Cleaning;
                        table.Notes = null;
                        table.ResTime = null;
                        await _tableRepository.UpdateAsync(table);
                    }
                }
            }

            catch (Exception)
            {
                TempData["Error"] = "Failed to create payment.";
                paymentViewModel.TotalAmount = await CalculateTotalAmount(paymentViewModel.OrderId);
                return PartialView("_CreatePaymentModal", paymentViewModel);
            }
            return Json(new { success = true });
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
                                    .Select(item => new OrderItemViewModel(item)).ToList();

            return PartialView("_DetailsPaymentModal", payment);
        }

		[HttpGet("DetailsByOrderId/{orderId}")]
		public async Task<IActionResult> DetailsByOrderId(long orderId)
		{
			var order = await _orderRepository.GetByIDAsync(orderId);
			if (order == null) return NotFound();

            var bill = (await _billRepository.GetAllAsync()).FirstOrDefault(b => b.OrderId == orderId);
            if (bill == null) return NotFound();

			var payment = new PaymentViewModel(bill)
			{
				OrderItems = (await _orderItemRepository.GetAllAsync())
									.Where(item => item.OrderId == bill.OrderId)
									.Select(item => new OrderItemViewModel(item)).ToList()
			};

			return PartialView("_DetailsPaymentModal", payment);
		}
	}
}