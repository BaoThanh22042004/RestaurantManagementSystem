using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
    public class OrderRepository : IOrderRepository, IDisposable
    {
        private readonly DBContext _context;

        public OrderRepository(DBContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            //  return await _context.Orders.Include(order => order.OrderItems)
            //.Include(order => order.Reservation)
            //.Include(order => order.Table)
            //.AsNoTracking()
            //.ToListAsync();

            string sqlGetAllOrders = "SELECT * FROM Orders";
            string sqlGetOrderItemsByOrderIds = "SELECT * FROM OrderItems WHERE OrderId IN ({0})";
            string sqlGetReservationsByIds = "SELECT * FROM Reservations WHERE ResId IN ({0})";
            string sqlGetTablesByIds = "SELECT * FROM Tables WHERE TableId IN ({0})";

            // Get all orders
            var orders = await _context.Orders.FromSqlRaw(sqlGetAllOrders).ToListAsync();

            // Include OrderIds
            var orderIds = orders.Select(o => o.OrderId).Distinct().ToList();
            if (orderIds.Count != 0)
            {
                var orderItems = await _context.OrderItems
                                         .FromSqlRaw(string.Format(sqlGetOrderItemsByOrderIds, string.Join(",", orderIds)))
                                         .ToListAsync();

                var orderItemsGrouped = orderItems.GroupBy(oi => oi.OrderId)
                                                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var order in orders)
                {
                    if (orderItemsGrouped.TryGetValue(order.OrderId, out var orderItemsList))
                    {
                        order.OrderItems = orderItemsList;
                    }
                }
            }


            // Include ReservationIds
            var reservationIds = orders.Select(o => o.ResId).Distinct().ToList();
            reservationIds.RemoveAll(r => !r.HasValue);
            if (reservationIds.Count != 0)
            {
                var reservations = await _context.Reservations
                                        .FromSqlRaw(string.Format(sqlGetReservationsByIds, string.Join(",", reservationIds)))
                                        .ToDictionaryAsync(r => r.ResId);

                foreach (var order in orders)
                {
                    if (order.ResId.HasValue)
                    {
                        if (reservations.TryGetValue(order.ResId.Value, out var reservation))
                        {
                            order.Reservation = reservation;
                        }
                    }
                }
            }


            // Include TableIds
            var tableIds = orders.Select(o => o.TableId).Distinct().ToList();
            tableIds.RemoveAll(t => !t.HasValue);
            if (tableIds.Count != 0)
            {
                var tables = await _context.Tables
                                    .FromSqlRaw(string.Format(sqlGetTablesByIds, string.Join(",", tableIds)))
                                    .ToDictionaryAsync(t => t.TableId);

                foreach (var order in orders)
                {
                    if (order.TableId.HasValue)
                    {
                        if (tables.TryGetValue(order.TableId.Value, out var table))
                        {
                            order.Table = table;
                        }
                    }
                }
            }

            return orders;
        }

        public async Task<Order?> GetByIDAsync(long id)
        {
            string sqlGetOrderById = "SELECT * FROM Orders WHERE OrderId = {0}";
            string sqlGetOrderItemsByOrderId = "SELECT * FROM OrderItems WHERE OrderId = {0}";
            string sqlGetReservationById = "SELECT * FROM Reservations WHERE ReservationId = {0}";
            string sqlGetTableById = "SELECT * FROM Tables WHERE TableId = {0}";
            string sqlGetUsersByIds = "SELECT * FROM Users WHERE UserId IN ({0})";

            // Get order by ID
            var order = await _context.Orders.FromSqlRaw(sqlGetOrderById, id).FirstOrDefaultAsync();

            if (order == null)
            {
                return null; // or handle the case where the order is not found
            }

            // Include OrderItems
            var orderItems = await _context.OrderItems
                                        .FromSqlRaw(string.Format(sqlGetOrderItemsByOrderId, order.OrderId))
                                        .ToListAsync();

            order.OrderItems = orderItems;

            // Include Reservation
            if (order.ResId.HasValue)
            {
                var reservation = await _context.Reservations
                                            .FromSqlRaw(string.Format(sqlGetReservationById, order.ResId.Value))
                                            .FirstOrDefaultAsync();
                order.Reservation = reservation;
            }

            // Include Table
            if (order.TableId.HasValue)
            {
                var table = await _context.Tables
                                    .FromSqlRaw(string.Format(sqlGetTableById, order.TableId.Value))
                                    .FirstOrDefaultAsync();
                order.Table = table;
            }

            // Include Users
            var userIds = order.OrderItems.Select(oi => oi.CreatedBy).Distinct().ToList();

            if (userIds.Count != 0)
            {
                var users = await _context.Users
                                    .FromSqlRaw(string.Format(sqlGetUsersByIds, string.Join(",", userIds)))
                                    .ToDictionaryAsync(u => u.UserId);

                foreach (var orderItem in order.OrderItems)
                {
                    if (users.TryGetValue(orderItem.CreatedBy, out var user))
                    {
                        orderItem.Creator = user;
                    }
                }
            }

            return order;
        }

        public async Task<Order> InsertAsync(Order order)
        {
            //var entityEntry = await _context.Orders.AddAsync(order);
            //await SaveAsync();
            //return entityEntry.Entity;

            string sqlInsertOrder = "INSERT INTO Orders (TableId, CreatedAt, Status, ResId) VALUES ({0}, {1}, {2}, {3});SELECT * FROM Orders WHERE OrderId = SCOPE_IDENTITY()";

            var insertedOrders = await _context.Orders.FromSqlRaw(sqlInsertOrder,
                order.TableId, order.CreatedAt, order.Status, order.ResId).ToListAsync();

            var orderInserted = insertedOrders.FirstOrDefault();

            if (orderInserted == null)
            {
                throw new Exception("Failed to insert order");
            }

            return orderInserted;

        }

        public async Task DeleteAsync(long id)
        {
            //var order = await _context.Orders.FindAsync(id);
            //if (order == null)
            //{
            //    throw new Exception($"Order with id {id} not found");
            //}
            //await DeleteAsync(order);

            string sqlGetOrderById = "SELECT * FROM Orders WHERE OrderId = {0}";

            var order = await _context.Orders.FromSqlRaw(sqlGetOrderById, id).FirstOrDefaultAsync();

            if (order == null)
            {
                throw new Exception($"Order with id {id} not found");
            }

            await DeleteAsync(order);
        }

        public async Task DeleteAsync(Order order)
        {
            //if (_context.Entry(order).State == EntityState.Detached)
            //{
            //    _context.Orders.Attach(order);
            //}
            //_context.Orders.Remove(order);
            //await SaveAsync();

            string sqlDeleteOrder = "DELETE FROM Orders WHERE OrderId = {0}";

            await _context.Database.ExecuteSqlRawAsync(string.Format(sqlDeleteOrder, order.OrderId));
        }

        public async Task UpdateAsync(Order order)
        {
            //_context.Orders.Attach(order);
            //_context.Entry(order).State = EntityState.Modified;
            //await SaveAsync();

            string sqlUpdateOrder = "UPDATE Orders SET TableId = {0}, CreatedAt = {1}, Status = {2}, ResId = {3} WHERE OrderId = {4}";

            await _context.Database.ExecuteSqlRawAsync(sqlUpdateOrder, order.TableId, order.CreatedAt, order.Status, order.ResId, order.OrderId);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
