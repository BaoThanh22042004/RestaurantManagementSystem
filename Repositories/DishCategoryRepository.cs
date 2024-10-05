using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
	public class DishCategoryRepository : IDishCategoryRepository, IDisposable
	{
		private readonly DBContext _context;

		public DishCategoryRepository(DBContext context)
		{
			_context = context;
		}
		public async Task<IEnumerable<DishCategory>> GetAllAsync()
		{
			return await _context.DishCategories.AsNoTracking().ToListAsync();
		}

		public async Task<DishCategory?> GetByIDAsync(int id)
		{
			return await _context.DishCategories.FindAsync(id);
		}

		public async Task InsertAsync(DishCategory dishCategory)
		{
			await _context.DishCategories.AddAsync(dishCategory);
			await SaveAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var dishCategory = await _context.DishCategories.FindAsync(id);
			if (dishCategory == null)
			{
				throw new Exception($"Dish Category with id {id} not found");
			}
			await DeleteAsync(dishCategory);
		}

		public async Task DeleteAsync(DishCategory dishCategory)
		{
			if (_context.Entry(dishCategory).State == EntityState.Detached)
			{
				_context.DishCategories.Attach(dishCategory);
			}
			_context.DishCategories.Remove(dishCategory);
			await SaveAsync();
		}

		public async Task UpdateAsync(DishCategory dishCategory)
		{
			_context.DishCategories.Attach(dishCategory);
			_context.Entry(dishCategory).State = EntityState.Modified;
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
