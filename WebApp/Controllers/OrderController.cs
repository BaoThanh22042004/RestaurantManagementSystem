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
            result = result.Where(category => dishList.Any(dish => dish.CategoryId.Equals(category.CatId)));
            return result.Select(category => new DishCategoryViewModel(category)).ToList();
        }

        private async Task<List<DishViewModel>> GetDishList()
        {
            var dishes = (await _dishRepository.GetAllAsync()).Where(d => d.Visible);
            return dishes.Select(d => new DishViewModel(d)).ToList();
        }

        private async Task<string> GetDishNameByDishId(int dishId)
        {
            var dish = await _dishRepository.GetByIDAsync(dishId);
            var dishName = dish.DishName;
            return dishName;
        }

        private async Task<List<SelectListItem>> GetTableStatusList()
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
                TableOptions = await GetTableStatusList()
            };

            return View("CreateOrderView", order);
        }

        [HttpPost]
        [Route("Create/{tableId}")]
        public async Task<IActionResult> Create(OrderViewModel orderViewModel)
        {
            if (!ModelState.IsValid)
            {
                orderViewModel.TableOptions = await GetTableStatusList();
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

        [Route("Details/{orderId}")]
        public async Task<IActionResult> Details(long orderId)
        {
            var order = await _orderRepository.GetByIDAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            var orderViewModel = new OrderViewModel(order);
            orderViewModel.TableOptions = await GetTableStatusList();

            foreach (var item in orderViewModel.OrderItems)
            {
                item.StatusOptions = await GetOrderItemStatusList();
                item.DishName = await GetDishNameByDishId(item.DishId);
            }
            return PartialView("_DetailsOrderModal", orderViewModel);
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
        public async Task<IActionResult> AddOrderItem(int orderId,int dishId )
        {
            var orderItem = new OrderItemViewModel
            {
                OrderId = orderId,
                DishId = dishId,
                DishName = await GetDishNameByDishId(dishId),
                StatusOptions = await GetOrderItemStatusList()
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
                orderItem.StatusOptions = await GetOrderItemStatusList();
                orderItem.DishName = await GetDishNameByDishId(dishId);
                return PartialView("_AddOrderItemView", orderItem);
            }

            var staffId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var dish = await _dishRepository.GetByIDAsync(orderItem.DishId);
            var dishPrice = dish.Price;

            var item = new OrderItem
            {
                OrderId = orderItem.OrderId,
                DishId = orderItem.DishId,
                CreatedBy = staffId,
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
                TableOptions = await GetTableStatusList()
            };

            foreach (var item in orderViewModel.OrderItems)
            {
                item.StatusOptions = await GetOrderItemStatusList();
                item.DishName = await GetDishNameByDishId(item.DishId);
            }
            return PartialView("_EditOrderModal", orderViewModel);
        }

        [HttpPost]
        [Route("Edit/{orderId}")]
        public async Task<IActionResult> Edit(OrderViewModel orderViewModel, long orderId)
        {
            if (!ModelState.IsValid)
            {
                orderViewModel.TableOptions = await GetTableStatusList();
                foreach (var item in orderViewModel.OrderItems)
                {
                    item.StatusOptions = await GetOrderItemStatusList();
                    item.DishName = await GetDishNameByDishId(item.DishId);
                }
                return PartialView("_DetailsOrderModal", orderViewModel);
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
                orderViewModel.TableOptions = await GetTableStatusList();
                foreach (var item in orderViewModel.OrderItems)
                {
                    item.StatusOptions = await GetOrderItemStatusList();
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
                TableOptions = await GetTableStatusList()
            };
            foreach (var item in orderViewModel.OrderItems)
            {
                item.StatusOptions = await GetOrderItemStatusList();
                item.DishName = await GetDishNameByDishId(item.DishId);
            }
            return PartialView("_DeleteOrderModal", orderViewModel);
        }

        [HttpPost]
        [Route("Delete/{orderId}")]
        public async Task<IActionResult> DeleteConfirmed(long orderId)
        {
            try
            {
                await _orderRepository.DeleteAsync(orderId);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting dish. Please try again later.";
                return RedirectToAction("Delete", new { orderId });
            }

            return Json(new { success = true });
        }
    }

}
