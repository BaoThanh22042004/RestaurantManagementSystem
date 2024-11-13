﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Repositories.Interface;
using System.Security.Claims;
using WebApp.Models;
using Table = Models.Entities.Table;

namespace WebApp.Controllers
{
	[Route("Dashboard/Order")]
	[Authorize(Roles = $"{nameof(Role.Manager)}")]
	public class OrderController : Controller
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IOrderItemRepository _orderItemRepository;
		private readonly IReservationRepository _reservationRepository;
		private readonly ITableRepository _tableRepository;
		private readonly IDishRepository _dishRepository;
		private readonly IDishCategoryRepository _dishCategoryRepository;

		public OrderController(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IReservationRepository reservationRepository, ITableRepository tableRepository, IDishRepository dishRepository, IDishCategoryRepository dishCategoryRepository)
		{
			_orderRepository = orderRepository;
			_orderItemRepository = orderItemRepository;
			_reservationRepository = reservationRepository;
			_tableRepository = tableRepository;
			_dishRepository = dishRepository;
			_dishCategoryRepository = dishCategoryRepository;
		}

		private async Task<List<DishCategoryViewModel>> GetCategoryList(List<DishViewModel> dishList)
		{
			var result = await _dishCategoryRepository.GetAllAsync();
			result = result.Where(category => dishList.Exists(dish => dish.CategoryId.Equals(category.CatId)));
			return result.Select(category => new DishCategoryViewModel(category)).ToList();
		}

		private async Task<List<DishViewModel>> GetDishList()
		{
			var dishes = (await _dishRepository.GetAllAsync()).Where(d => d.Visible);
			return dishes.Select(d => new DishViewModel(d)).ToList();
		}

		//private async Task<string> GetDishNameByDishId(int dishId)
		//{
		//    var dish = await _dishRepository.GetByIDAsync(dishId);
		//    var dishName = dish.DishName;
		//    return dishName;
		//}

		private async Task<List<Table>> GetAvailableTables()
		{
			var tables = (await _tableRepository.GetAllAsync())
				.Where(t => t.Status == TableStatus.Available)
				.ToList();

			return tables;
		}

		private static List<SelectListItem> GetOrderItemStatusList()
		{
			var orderItemStatuses = Enum.GetValues(typeof(OrderItemStatus))
				.Cast<OrderItemStatus>()
				.Select(s => new SelectListItem
				{
					Value = ((int)s).ToString(),
					Text = s.ToString()
				}).ToList();

			return orderItemStatuses;
		}

		public async Task<IActionResult> Index()
		{
			var orders = await _orderRepository.GetAllAsync();
			var orderList = orders.Select(order => new OrderViewModel(order)).ToList();
			return View("OrderView", orderList);
		}

		[HttpGet("Create")]
		public async Task<IActionResult> Create(int? tableId, long? reservationId)
		{
			if (tableId == null && reservationId == null)
			{
				return BadRequest();
			}

			var table = (await GetAvailableTables()).Find(t => t.TableId == tableId);
			var reservation = reservationId.HasValue ? await _reservationRepository.GetByIDAsync(reservationId.Value) : null;
			var order = new OrderViewModel
			{
				TableId = tableId,
				Table = table,
				ResId = reservationId,
				Reservation = reservation,
			};
			return PartialView("_CreateOrderModal", order);
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create(OrderViewModel orderViewModel)
		{
			Order orderEntity;
			if (!ModelState.IsValid)
			{
				orderViewModel.Table = (await GetAvailableTables()).Find(t => t.TableId == orderViewModel.TableId);
				orderViewModel.Reservation = orderViewModel.ResId.HasValue ? await _reservationRepository.GetByIDAsync(orderViewModel.ResId.Value) : null;
				return PartialView("_CreateOrderModal", orderViewModel);
			}

			try
			{
				OrderStatus orderStatus;
				if (orderViewModel.ResId.HasValue)
				{
					orderStatus = OrderStatus.Reservation;
				}
				else
				{
					orderStatus = OrderStatus.Serving;
				}

				var order = new Order
				{
					TableId = orderViewModel.TableId,
					ResId = orderViewModel.ResId,
					Status = orderStatus,
					CreatedAt = DateTime.Now,
				};

				orderEntity = await _orderRepository.InsertAsync(order);

				if (orderViewModel.TableId.HasValue)
				{
					var table = await _tableRepository.GetByIDAsync(orderViewModel.TableId.Value);
					if (table != null)
					{
						table.Status = TableStatus.Occupied;
						await _tableRepository.UpdateAsync(table);
					}
				}
			}
			catch (Exception e)
			{
				TempData["Error"] = "An error occurred while creating the order. Please try again later.";
				orderViewModel.Table = (await GetAvailableTables()).Find(t => t.TableId == orderViewModel.TableId);
				orderViewModel.Reservation = orderViewModel.ResId.HasValue ? await _reservationRepository.GetByIDAsync(orderViewModel.ResId.Value) : null;
				return PartialView("_CreateOrderModal", orderViewModel);
			}

			return RedirectToAction("Details", new { orderId = orderEntity.OrderId });
		}

		[HttpGet("Details/{orderId}")]
		public async Task<IActionResult> Details(long orderId)
		{
			var order = await _orderRepository.GetByIDAsync(orderId);
			if (order == null)
			{
				return NotFound();
			}
			var orderViewModel = new OrderViewModel(order);

			return View("DetailsOrderView", orderViewModel);
		}

		[Route("Details/{orderId}/OrderItemMenu")]
		public async Task<IActionResult> OrderItemMenu(OrderMenuViewModel orderMenuViewModel, long orderId)
		{
			orderMenuViewModel.OrderId = orderId;
			orderMenuViewModel.Dishes ??= await GetDishList();
			orderMenuViewModel.Categories ??= await GetCategoryList(orderMenuViewModel.Dishes);
			orderMenuViewModel.SelectedCategories ??= new List<int>();

			if (string.IsNullOrWhiteSpace(orderMenuViewModel.Keyword) && orderMenuViewModel.SelectedCategories.Count == 0)
			{
				return PartialView("_OrderItemMenu", orderMenuViewModel);
			}

			if (!string.IsNullOrWhiteSpace(orderMenuViewModel.Keyword))
			{
				orderMenuViewModel.Dishes = orderMenuViewModel.Dishes.Where(d => d.DishName.Contains(orderMenuViewModel.Keyword, StringComparison.OrdinalIgnoreCase)).ToList();
			}

			if (orderMenuViewModel.SelectedCategories.Count != 0)
			{
				orderMenuViewModel.SelectedCategories.ForEach(selected => orderMenuViewModel.Categories.Find(c => c.CategoryId.Equals(selected)).IsSelected = true);
				orderMenuViewModel.Dishes = orderMenuViewModel.Dishes.Where(d => orderMenuViewModel.SelectedCategories.Contains(d.CategoryId)).ToList();
			}

			return PartialView("_OrderItemMenu", orderMenuViewModel);
		}

		[Route("Details/{orderId}/OrderItemMenu/AddOrderItem/{dishId}")]
		public async Task<IActionResult> AddOrderItem(int orderId, int dishId)
		{
			var orderItem = new OrderItemViewModel
			{
				OrderId = orderId,
				DishId = dishId,
				StatusOptions = GetOrderItemStatusList()
			};

			return PartialView("_AddOrderItemModal", orderItem);
		}

		[Route("Details/{orderId}/OrderItemMenu/AddOrderItem/{dishId}")]
		[HttpPost]
		public async Task<IActionResult> AddOrderItem(OrderItemViewModel orderItem, int dishId)
		{
			if (!ModelState.IsValid)
			{
				orderItem.DishId = dishId;
				orderItem.StatusOptions = GetOrderItemStatusList();
				return PartialView("_AddOrderItemView", orderItem);
			}

			var dish = await _dishRepository.GetByIDAsync(orderItem.DishId);
			var dishPrice = dish.Price;

			var item = new OrderItem
			{
				OrderId = orderItem.OrderId,
				DishId = orderItem.DishId,
				CreatedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
				Quantity = orderItem.Quantity,
				Price = dishPrice,
				Status = OrderItemStatus.Pending,
				Notes = orderItem.Notes
			};

			var order = await _orderRepository.GetByIDAsync(orderItem.OrderId);
			order.OrderItems.Add(item);
			await _orderItemRepository.InsertAsync(item);
			await _orderRepository.UpdateAsync(order);

			return RedirectToAction("Details", new { orderItem.OrderId });
		}

		[Route("Edit/{orderId}")]
		public async Task<IActionResult> Edit(long orderId)
		{
			var order = await _orderRepository.GetByIDAsync(orderId);
			if (order == null)
			{
				return NotFound();
			}

			var orderViewModel = new OrderViewModel(order)
			{
				TableOptions = await GetAvailableTables()
			};

			foreach (var item in orderViewModel.OrderItems)
			{
				item.StatusOptions = GetOrderItemStatusList();
			}
			return PartialView("_EditOrderModal", orderViewModel);
		}

		[HttpPost("Edit/{orderId}")]
		public async Task<IActionResult> Edit(OrderViewModel orderViewModel, long orderId)
		{
			if (!ModelState.IsValid)
			{
				orderViewModel.TableOptions = await GetAvailableTables();
				foreach (var item in orderViewModel.OrderItems)
				{
					item.StatusOptions = GetOrderItemStatusList();
				}
				return PartialView("_EditOrderModal", orderViewModel);
			}

			try
			{
				var order = await _orderRepository.GetByIDAsync(orderId);
				order.TableId = orderViewModel.TableId;
				order.ResId = orderViewModel.ResId;
				order.Status = orderViewModel.Status;

				foreach (var viewModelItem in orderViewModel.OrderItems)
				{
					var Item = order.OrderItems.FirstOrDefault(i => i.OrItemId == viewModelItem.OrItemId);
					Item.Status = viewModelItem.Status;
					Item.Notes = viewModelItem.Notes;
					await _orderItemRepository.UpdateAsync(Item);
				}

				await _orderRepository.UpdateAsync(order);
			}
			catch (InvalidOperationException)
			{
				return NotFound();
			}
			catch (Exception e)
			{
				TempData["Error"] = "An error occurred while updating the order. Please try again later.";
				orderViewModel.TableOptions = await GetAvailableTables();
				foreach (var item in orderViewModel.OrderItems)
				{
					item.StatusOptions = GetOrderItemStatusList();
				}
				return PartialView("_EditOrderModal", orderViewModel);
			}
			return Json(new { success = true });
		}

		[Route("Delete/{orderId}")]
		public async Task<IActionResult> Delete(long orderId)
		{
			var order = await _orderRepository.GetByIDAsync(orderId);
			if (order == null)
			{
				return NotFound();
			}
			var orderViewModel = new OrderViewModel(order)
			{
				TableOptions = await GetAvailableTables()
			};
			foreach (var item in orderViewModel.OrderItems)
			{
				item.StatusOptions = GetOrderItemStatusList();
			}
			return PartialView("_DeleteOrderModal", orderViewModel);
		}

		[HttpPost]
		[Route("Delete/{orderId}")]
		public async Task<IActionResult> DeleteConfirmed(long orderId)
		{
			try
			{
				var order = await _orderRepository.GetByIDAsync(orderId);
				if (order == null)
				{
					return NotFound();
				}
				if (order.OrderItems.Any())
				{
					TempData["Error"] = "Cannot delete the order because it contains order items.";
					return RedirectToAction("Delete", new { orderId });
				}
				await _orderRepository.DeleteAsync(orderId);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while deleting dish. Please try again later.";
				return RedirectToAction("Delete", new { orderId });
			}

			return Json(new { success = true });
		}

		[HttpPost]
		[Route("Details/{orderId}/DeleteOrderItem/{orderItemId}")]
		public async Task<IActionResult> DeleteOrderItemConfirmed(long orderId, long orderItemId)
		{
			try
			{
				var orderItem = await _orderItemRepository.GetByIDAsync(orderItemId);
				if (orderItem == null)
				{
					return Json(new { success = false, error = "Order item not found." });
				}

				if (orderItem.Status != OrderItemStatus.Pending)
				{
					return Json(new { success = false, error = "Cannot delete this order item because its status is not Pending." });
				}

				await _orderItemRepository.DeleteAsync(orderItemId);

				return Json(new { success = true });
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while deleting the order item. Please try again later.";
				return Json(new { success = false, error = TempData["Error"] });
			}
		}
	}
}
