using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
    public class OrderItemRepository : IOrderItemRepository, IDisposable
    {
        private readonly DBContext _context;

        public OrderItemRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderItem>> GetAllAsync()
        {
            //return await _context.OrderItems.Include(oi => oi.Dish).AsNoTracking().ToListAsync();

            string sqlGetAllOrderItems = "SELECT * FROM OrderItems";
            string sqlGetDishesByIds = "SELECT * FROM Dishes WHERE DishId IN ({0})";

            var orderItems = await _context.OrderItems.FromSqlRaw(sqlGetAllOrderItems).ToListAsync();

            // Include dishes
            var dishIds = orderItems.Select(oi => oi.DishId);
            var dishes = await _context.Dishes.FromSqlRaw(string.Format(sqlGetDishesByIds, string.Join(',', dishIds))).ToDictionaryAsync(d => d.DishId);

            foreach (var orderItem in orderItems)
            {
                if (dishes.TryGetValue(orderItem.DishId, out var dish))
                {
                    orderItem.Dish = dish;
                }
            }

            return orderItems;
        }

        public async Task<OrderItem?> GetByIDAsync(long id)
        {
            //return await _context.OrderItems.Include(oi => oi.Dish).FirstOrDefaultAsync(oi => oi.OrItemId == id);

            string sqlGetOrderItemById = "SELECT * FROM OrderItems WHERE OrItemId = {0}";
            string sqlGetDishById = "SELECT * FROM Dishes WHERE DishId = {0}";

            // Get order item by ID
            var orderItem = await _context.OrderItems.FromSqlRaw(sqlGetOrderItemById, id).FirstOrDefaultAsync();
            return orderItem;
        }

        public async Task InsertAsync(OrderItem orderItem)
        {
            //await _context.OrderItems.AddAsync(orderItem);
            //await SaveAsync();
            string sqlInsertOrderItem = "INSERT INTO OrderItems (OrderId, DishId, CreatedBy, Quantity, Price, Status, Notes) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})";
            await _context.Database.ExecuteSqlRawAsync(sqlInsertOrderItem,
                orderItem.OrderId, orderItem.DishId, orderItem.CreatedBy, orderItem.Quantity, orderItem.Price, orderItem.Status, orderItem.Notes);
        }

        public async Task DeleteAsync(long id)
        {
            //var orderItem = await _context.OrderItems.FindAsync(id);
            //if (orderItem == null)
            //{
            //    throw new Exception($"Order Item with id {id} not found");
            //}
            //await DeleteAsync(orderItem);

            string sqlGetOrderItemById = "SELECT * FROM OrderItems WHERE OrItemId = {0}";

            var orderItem = await _context.OrderItems
                                          .FromSqlRaw(sqlGetOrderItemById, id)
                                          .FirstOrDefaultAsync();
            if (orderItem == null)
            {
                throw new Exception($"Order Item with id {id} not found");
            }

            await DeleteAsync(orderItem);
        }

        public async Task DeleteAsync(OrderItem orderItem)
        {
            //if (_context.Entry(orderItem).State == EntityState.Detached)
            //{
            //    _context.OrderItems.Attach(orderItem);
            //}
            //_context.OrderItems.Remove(orderItem);

            string sqlDeleteOrderItem = "DELETE FROM OrderItems WHERE OrItemId = {0}";

            await _context.Database.ExecuteSqlRawAsync(sqlDeleteOrderItem, orderItem.OrItemId);
        }

        public async Task UpdateAsync(OrderItem orderItem)
        {
            //_context.OrderItems.Attach(orderItem);
            //_context.Entry(orderItem).State = EntityState.Modified;

            string sqlUpdateOrderItem = "UPDATE OrderItems SET OrderId = {0}, DishId = {1}, CreatedBy = {2}, Quantity = {3}, Price = {4}, Status = {5}, Notes = {6} WHERE OrItemId = {7}";

            await _context.Database.ExecuteSqlRawAsync(sqlUpdateOrderItem,
                orderItem.OrderId, orderItem.DishId, orderItem.CreatedBy, orderItem.Quantity, orderItem.Price, orderItem.Status, orderItem.Notes, orderItem.OrItemId);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
