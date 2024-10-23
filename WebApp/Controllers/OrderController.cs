using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models.Entities;
using Repositories;
using Repositories.Interface;
using System.Linq;
using System.Security.Claims;
using WebApp.Models;

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

        public OrderController(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IReservationRepository reservationRepository, ITableRepository tableRepository, IDishRepository dishRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _reservationRepository = reservationRepository;
            _tableRepository = tableRepository;
            _dishRepository = dishRepository;
        }

        private async Task<List<DishViewModel>> GetDishList()
        {
            var dishes = (await _dishRepository.GetAllAsync());
            return dishes.Where(d => d.Visible).Select(d => new DishViewModel(d)).ToList();
        }

        private async Task<List<SelectListItem>> GetTableList()
        {
            var tables = (await _tableRepository.GetAllAsync())
                .Where(t => t.Status == TableStatus.Available)
                .Select(t => new SelectListItem
                {
                    Value = t.TableId.ToString(),
                    Text = t.TableName
                })
                .ToList();

            return tables;
        }

        private async Task<List<SelectListItem>> GetOrderItemStatusList()
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

        [Route("Create/{tableId}")]
        public async Task<IActionResult> Create(int tableId)
        {
            var order = new OrderViewModel
            {
                TableId = tableId,
                TableOptions = await GetTableList()
            };

            return View("CreateOrderView", order);
        }

        [HttpPost]
        [Route("Create/{tableId}")]
        public async Task<IActionResult> Create(OrderViewModel orderViewModel)
        {
            if (!ModelState.IsValid)
            {
                orderViewModel.TableOptions = await GetTableList();
                return View("CreateOrderView", orderViewModel);
            }
            try
            {
                var order = new Order
                {
                    TableId = orderViewModel.TableId,
                    ResId = orderViewModel.ResId,
                    CreatedAt = DateTime.Now,
                    Status = OrderStatus.Serving,
                    OrderItems = new List<OrderItem>()
                };
                await _orderRepository.InsertAsync(order);
            }
            catch (ArgumentException e)
            {
                TempData["Error"] = e.Message;
                return View("CreateOrderView", orderViewModel);
            }
            catch (Exception e)
            {
                TempData["Error"] = "An error occurred while creating the order. Please try again later.";
                return View("CreateOrderView", orderViewModel);
            }

            return RedirectToAction("Index");
        }

        [Route("Details/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            var order = await _orderRepository.GetByIDAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            var orderViewModel = new OrderViewModel(order);
            orderViewModel.TableOptions = await GetTableList();

            foreach (var item in orderViewModel.OrderItems)
            {
                item.StatusOptions = await GetOrderItemStatusList();
            }
            return View("DetailsOrderView", orderViewModel);
        }

        [Route("Details/{id}/AddOrderItem")]
        public async Task<IActionResult> AddOrderItem(long id)
        {
            var orderItemViewModel = new OrderItemViewModel
            {
                OrderId = id,
                Dishes = await GetDishList(),
                StatusOptions = await GetOrderItemStatusList()
            };

            return View("AddOrderItemView", orderItemViewModel);
        }

        [Route("Details/{id}/AddOrderItem")]
        [HttpPost]
        public async Task<IActionResult> AddOrderItem(OrderItemViewModel orderItem, long id)
        {
            if (!ModelState.IsValid)
            {
                orderItem.OrderId = id;
                orderItem.Dishes = await GetDishList();
                orderItem.StatusOptions = await GetOrderItemStatusList();
                return View("AddOrderItemView", orderItem);
            }

            var staffId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var dish = await _dishRepository.GetByIDAsync(orderItem.DishId);
            var dishPrice = dish.Price;

            var item = new OrderItem
            {
                OrderId = id,
                DishId = orderItem.DishId,
                CreatedBy = staffId,
                Quantity = orderItem.Quantity,
                Price = dishPrice,
                Status = OrderItemStatus.Pending,
                Notes = orderItem.Notes
            };

            var order = await _orderRepository.GetByIDAsync(id);
            order.OrderItems.Add(item);
            await _orderRepository.UpdateAsync(order);

            return RedirectToAction("Details", new { id });
        }

        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            var order = await _orderRepository.GetByIDAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var orderViewModel = new OrderViewModel(order)
            {
                TableOptions = await GetTableList()
            };

            foreach (var item in orderViewModel.OrderItems)
            {
                item.StatusOptions = await GetOrderItemStatusList();
            }
            return View("EditOrderView", orderViewModel);
        }

        [HttpPost]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(OrderViewModel orderViewModel, long id)
        {
            if (!ModelState.IsValid)
            {
                orderViewModel.TableOptions = await GetTableList();
                foreach (var item in orderViewModel.OrderItems)
                {
                    item.StatusOptions = await GetOrderItemStatusList();
                }
                return View("DetailsOrderView", orderViewModel);
            }

            try
            {
                var order = await _orderRepository.GetByIDAsync(id);
                order.TableId = orderViewModel.TableId;
                order.ResId = orderViewModel.ResId;
                order.Status = orderViewModel.Status;

                foreach (var viewModelItem in orderViewModel.OrderItems)
                {
                    var Item = order.OrderItems.FirstOrDefault(i => i.OrItemId == viewModelItem.OrItemId);
                    Item.Status = viewModelItem.Status;
                    Item.Notes = viewModelItem.Notes;
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
                orderViewModel.TableOptions = await GetTableList();
                foreach (var item in orderViewModel.OrderItems)
                {
                    item.StatusOptions = await GetOrderItemStatusList();
                }
                return View("EditOrderView", orderViewModel);
            }
            return RedirectToAction("Details", new {id});
        }

        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var order = await _orderRepository.GetByIDAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            var orderViewModel = new OrderViewModel(order)
            {
                TableOptions = await GetTableList()
            };
            foreach (var item in orderViewModel.OrderItems)
            {
                item.StatusOptions = await GetOrderItemStatusList();
            }
            return View("DeleteOrderView", orderViewModel);
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                await _orderRepository.DeleteAsync(id);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting dish. Please try again later.";
                return RedirectToAction("Delete", new { id });
            }

            return RedirectToAction("Index");
        }
    }
}
