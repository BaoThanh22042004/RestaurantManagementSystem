using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Utilities;

namespace WebApp.Controllers
{
	[Route("Reservation")]
	[Authorize(Roles = $"{nameof(Role.Customer)}, {nameof(Role.Waitstaff)}, {nameof(Role.Manager)}")]
	public class ReservationController : Controller
	{
		private readonly IReservationRepository _reservationRepository;
		private readonly IUserRepository _userRepository;
		private readonly CartManager _cartManager;
		private readonly IOrderRepository _orderRepository;
		private readonly IOrderItemRepository _orderItemRepository;

		public ReservationController(IReservationRepository reservationRepository, IUserRepository userRepository, CartManager cartManager, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
		{
			_reservationRepository = reservationRepository;
			_userRepository = userRepository;
			_cartManager = cartManager;
			_orderRepository = orderRepository;
			_orderItemRepository = orderItemRepository;
		}

		[HttpGet]
		public IActionResult MakeReservation()
		{
			var cart = _cartManager.GetCartFromCookie(Request);
			var makeReservation = new MakeReservationViewModel
			{
				Cart = cart,
				ResDate = DateOnly.FromDateTime(DateTime.Now),
				ResTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(1))
			};
			return View("MakeReservationView", makeReservation);
		}

		[HttpPost]
		public async Task<IActionResult> MakeReservation(MakeReservationViewModel makeReservation)
		{
			if (!ModelState.IsValid)
			{
				makeReservation.Cart = _cartManager.GetCartFromCookie(Request);
				return View("MakeReservationView", makeReservation);
			}

			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception();
				var customerId = int.Parse(userId);
				var reservation = new Reservation
				{
					CustomerId = customerId,
					PartySize = makeReservation.PartySize,
					ResDate = makeReservation.ResDate,
					ResTime = makeReservation.ResTime,
					Status = ReservationStatus.Pending,
					Notes = makeReservation.Notes,
				};

				var resEntity = await _reservationRepository.InsertAsync(reservation);

				var cart = _cartManager.GetCartFromCookie(Request);
				if (cart != null && cart.ItemList.Count != 0)
				{
					// Create order
					var order = new Order
					{
						CreatedAt = DateTime.Now,
						Status = OrderStatus.Reservation,
						ResId = resEntity.ResId,
					};

					var orderEntity = await _orderRepository.InsertAsync(order);

					// Create order item
					foreach (var item in cart.ItemList)
					{
						var orderItem = new OrderItem
						{
							OrderId = orderEntity.OrderId,
							CreatedBy = customerId,
							DishId = item.Id,
							Quantity = item.Quantity,
							Price = item.Price,
							Status = OrderItemStatus.Reservation,
						};

						await _orderItemRepository.InsertAsync(orderItem);
					}
				}
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while creating the reservation. Please try again later.";
				makeReservation.Cart = _cartManager.GetCartFromCookie(Request);
				return View("MakeReservationView", makeReservation);
			}

			// Clear the cart
			_cartManager.SaveCartToCookie(new CartViewModel(), Response);

			TempData["Success"] = "Reservation has been created successfully.";
			return RedirectToAction("MakeReservation");
		}

		[HttpGet("List")]
		public async Task<IActionResult> List()
		{
			var reservations = await _reservationRepository.GetAllAsync();
			var reservationList = reservations.Select(reservation => new ReservationViewModel(reservation));
			return View("ReservationView", reservationList);
		}

		[HttpGet("Create")]
		public async Task<IActionResult> Create()
		{
			var reservationViewModel = new ReservationViewModel()
			{
				Customers = await GetCustomerList(),
				ResDate = DateOnly.FromDateTime(DateTime.Now),
				ResTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(1))
			};
			return PartialView("_CreateReservationModal", reservationViewModel);
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create(ReservationViewModel reservationViewModel)
		{
			if (!ModelState.IsValid)
			{
				reservationViewModel.Customers = await GetCustomerList();
				return PartialView("_CreateReservationModal", reservationViewModel);
			}

			try
			{
				var reservation = new Reservation
				{
					CustomerId = reservationViewModel.CustomerId,
					PartySize = reservationViewModel.PartySize,
					ResDate = reservationViewModel.ResDate,
					ResTime = reservationViewModel.ResTime,
					Status = reservationViewModel.Status,
					DepositAmount = reservationViewModel.DepositAmount,
					Notes = reservationViewModel.Notes,
				};

				await _reservationRepository.InsertAsync(reservation);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while creating the reservation. Please try again later.";
				reservationViewModel.Customers = await GetCustomerList();
				return PartialView("_CreateReservationModal", reservationViewModel);
			}

			return Json(new { success = true });
		}

		private async Task<IEnumerable<User>> GetCustomerList()
		{
			return (await _userRepository.GetAllAsync()).Where(c => c.Role == Role.Customer);
		}

		[HttpGet("Details/{id}")]
		public async Task<IActionResult> Details(long id)
		{
			var reservation = await _reservationRepository.GetByIDAsync(id);
			if (reservation == null)
			{
				return NotFound();
			}

			var reservationViewModel = new ReservationViewModel(reservation);
			return PartialView("_DetailsReservationModal", reservationViewModel);
		}

		[HttpGet("Edit/{id}")]
		public async Task<IActionResult> Edit(long id)
		{
			var reservation = await _reservationRepository.GetByIDAsync(id);
			if (reservation == null)
			{
				return NotFound();
			}

			var reservationViewModel = new ReservationViewModel(reservation);
			return PartialView("_EditReservationModal", reservationViewModel);
		}

		[HttpPost("Edit/{ResId}")]
		public async Task<IActionResult> Edit(ReservationViewModel reservationViewModel, long ResId)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_EditReservationModal", reservationViewModel);
			}

			try
			{
				var reservationEntity = await _reservationRepository.GetByIDAsync(ResId);
				if (reservationEntity == null)
				{
					return NotFound();
				}

				reservationEntity.PartySize = reservationViewModel.PartySize;
				reservationEntity.ResDate = reservationViewModel.ResDate;
				reservationEntity.ResTime = reservationViewModel.ResTime;
				reservationEntity.Status = reservationViewModel.Status;
				if (reservationViewModel.Status == ReservationStatus.Cancelled)
				{
					var order = await _orderRepository.GetByIDAsync(reservationEntity.Order?.OrderId ?? -1);
					if (order != null)
					{
						order.Status = OrderStatus.Cancelled;
						await _orderRepository.UpdateAsync(order);
					}
				}
				reservationEntity.DepositAmount = reservationViewModel.DepositAmount;
				reservationEntity.Notes = reservationViewModel.Notes;

				await _reservationRepository.UpdateAsync(reservationEntity);
			}
			catch (Exception e)
			{
				TempData["Error"] = "An error occurred while updating the reservation. Please try again later.";
				return PartialView("_EditReservationModal", reservationViewModel);
			}
			return Json(new { success = true });
		}

		[HttpGet("Reservation/Delete/{id}")]
		public async Task<IActionResult> Delete(long id)
		{
			var reservation = await _reservationRepository.GetByIDAsync(id);
			if (reservation == null)
			{
				return NotFound();
			}

			var reservationViewModel = new ReservationViewModel(reservation) { Customers = await GetCustomerList() };
			return PartialView("_DeleteReservationModal", reservationViewModel);
		}

		[HttpPost("Reservation/Delete/{ResId}")]
		public async Task<IActionResult> DeleteConfirmed(long ResId)
		{
			try
			{
				await _reservationRepository.DeleteAsync(ResId);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while deleting the reservation. Please try again later.";
				return RedirectToAction("Delete", new { id = ResId });
			}

			return Json(new { success = true });
		}
	}
}
