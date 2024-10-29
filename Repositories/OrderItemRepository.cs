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
            return await _context.OrderItems.Include(oi => oi.Dish).AsNoTracking().ToListAsync();
        }

        public async Task<OrderItem?> GetByIDAsync(long id)
        {
            return await _context.OrderItems.Include(oi => oi.Dish).FirstOrDefaultAsync(oi => oi.OrItemId == id);
        }

        public async Task InsertAsync(OrderItem orderItem)
        {
            await _context.OrderItems.AddAsync(orderItem);
            await SaveAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                throw new Exception($"Order Item with id {id} not found");
            }
            await DeleteAsync(orderItem);
        }

        public async Task DeleteAsync(OrderItem orderItem)
        {
            if (_context.Entry(orderItem).State == EntityState.Detached)
            {
                _context.OrderItems.Attach(orderItem);
            }
            _context.OrderItems.Remove(orderItem);
            await SaveAsync();
        }

        public async Task UpdateAsync(OrderItem orderItem)
        {
            _context.OrderItems.Attach(orderItem);
            _context.Entry(orderItem).State = EntityState.Modified;
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
