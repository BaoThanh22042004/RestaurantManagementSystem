using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
	public class DishRepository : IDishRepository, IDisposable
	{
		private readonly DBContext _context;

		public DishRepository(DBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Dish>> GetAllAsync()
		{
			return await _context.Dishes.AsNoTracking().ToListAsync();
		}

		public async Task<Dish?> GetByIDAsync(int id)
		{
			return await _context.Dishes.FindAsync(id);
		}

		public async Task InsertAsync(Dish dish)
		{
			await _context.Dishes.AddAsync(dish);
			await SaveAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var dish = await _context.Dishes.FindAsync(id);
			if (dish == null)
			{
				throw new Exception($"Dish with id {id} not found");
			}
			await DeleteAsync(dish);
		}

		public async Task DeleteAsync(Dish dish)
		{
			if (_context.Entry(dish).State == EntityState.Detached)
			{
				_context.Dishes.Attach(dish);
			}
			_context.Dishes.Remove(dish);
			await SaveAsync();
		}

		public async Task UpdateAsync(Dish dish)
		{
			_context.Dishes.Attach(dish);
			_context.Entry(dish).State = EntityState.Modified;
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
