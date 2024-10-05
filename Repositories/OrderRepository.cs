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
			return await _context.Orders.AsNoTracking().ToListAsync();
		}

		public async Task<Order?> GetByIDAsync(long id)
		{
			return await _context.Orders.FindAsync(id);
		}

		public async Task InsertAsync(Order order)
		{
			await _context.Orders.AddAsync(order);
			await SaveAsync();
		}

		public async Task DeleteAsync(long id)
		{
			var order = await _context.Orders.FindAsync(id);
			if (order == null)
			{
				throw new Exception($"Order with id {id} not found");
			}
			await DeleteAsync(order);
		}

		public async Task DeleteAsync(Order order)
		{
			if (_context.Entry(order).State == EntityState.Detached)
			{
				_context.Orders.Attach(order);
			}
			_context.Orders.Remove(order);
			await SaveAsync();
		}

		public async Task UpdateAsync(Order order)
		{
			_context.Orders.Attach(order);
			_context.Entry(order).State = EntityState.Modified;
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
