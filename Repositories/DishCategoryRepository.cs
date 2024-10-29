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
			string sqlGetAllDishCategories = "SELECT * FROM DishCategories";
			return await _context.DishCategories.FromSqlRaw(sqlGetAllDishCategories).ToListAsync();
		}

		public async Task<DishCategory?> GetByIDAsync(int id)
		{
			string sqlGetDishCategoryById = "SELECT * FROM DishCategories WHERE CatId = {0}";
			return await _context.DishCategories.FromSqlRaw(sqlGetDishCategoryById, id).FirstOrDefaultAsync();
		}

		public async Task<DishCategory> InsertAsync(DishCategory dishCategory)
		{
			string sqlGetDishCategoryByName = "SELECT * FROM DishCategories WHERE CatName = {0}";
			string sqlInsertDishCategory = @"INSERT INTO DishCategories (CatName)
											VALUES ({0});
											SELECT * FROM DishCategories WHERE CatId = SCOPE_IDENTITY();";

			var isContainDishCategory = await _context.DishCategories.FromSqlRaw(sqlGetDishCategoryByName, dishCategory.CatName).AnyAsync();
			if (isContainDishCategory)
			{
				throw new ArgumentException($"Dish Category {dishCategory.CatName} is already exist");
			}

			var insertedDishCategories = await _context.DishCategories.FromSqlRaw(sqlInsertDishCategory, dishCategory.CatName).ToListAsync();

			var dishCategoryInserted = insertedDishCategories.FirstOrDefault();

			if (dishCategoryInserted == null)
			{
				throw new Exception("Failed to insert dish category");
			}

			return dishCategoryInserted;
		}

		public async Task DeleteAsync(int id)
		{
			string sqlGetDishCategoryById = "SELECT * FROM DishCategories WHERE CatId = {0}";

			var dishCategory = await _context.DishCategories.FromSqlRaw(sqlGetDishCategoryById, id).FirstOrDefaultAsync();
			if (dishCategory == null)
			{
				throw new ArgumentException($"Dish Category with id {id} not found");
			}

			await DeleteAsync(dishCategory);
		}

		public async Task DeleteAsync(DishCategory dishCategory)
		{
			string sqlDeleteDishCategory = "DELETE FROM DishCategories WHERE CatId = {0}";
			await _context.Database.ExecuteSqlRawAsync(sqlDeleteDishCategory, dishCategory.CatId);
		}

		public async Task UpdateAsync(DishCategory dishCategory)
		{
			string sqlUpdateDishCategory = "UPDATE DishCategories SET CatName = {0} WHERE CatId = {1}";
			await _context.Database.ExecuteSqlRawAsync(sqlUpdateDishCategory, dishCategory.CatName, dishCategory.CatId);
		}

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
