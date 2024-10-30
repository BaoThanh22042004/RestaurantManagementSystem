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
			string sqlGetAllDishes = "SELECT * FROM Dishes";
			string sqlGetDishCategoriesByIds = "SELECT * FROM DishCategories WHERE CatId IN ({0})";

			// Get all dishes
			var dishes = await _context.Dishes.FromSqlRaw(sqlGetAllDishes).ToListAsync();

			// Include categories
			var categoryIds = dishes.Select(d => d.CategoryId).Distinct().ToList();
			var categories = await _context.DishCategories
											.FromSqlRaw(string.Format(sqlGetDishCategoriesByIds, string.Join(",", categoryIds)))
											.ToDictionaryAsync(c => c.CatId);
			foreach (var dish in dishes)
			{
				if (categories.TryGetValue(dish.CategoryId, out var category))
				{
					dish.Category = category;
				}
			}

			return dishes;
		}

		public async Task<Dish?> GetByIDAsync(int id)
		{
			string sqlGetDishById = "SELECT * FROM Dishes WHERE DishId = {0}";
			string sqlGetDishCategoryById = "SELECT * FROM DishCategories WHERE CatId = {0}";

			// Get dish by ID
			var dish = await _context.Dishes.FromSqlRaw(sqlGetDishById, id).FirstOrDefaultAsync();

			if (dish == null)
			{
				return null;
			}

			// Include category
			var category = await _context.DishCategories
										.FromSqlRaw(sqlGetDishCategoryById, dish.CategoryId)
										.FirstOrDefaultAsync();
			if (category != null)
			{
				dish.Category = category;
			}

			return dish;
		}

		public async Task<Dish> InsertAsync(Dish dish)
		{
			string sqlInsertDish = @"INSERT INTO Dishes (DishName, Price, Description, Visible, CategoryId)
									VALUES ({0}, {1}, {2}, {3}, {4});
									SELECT * FROM Dishes WHERE DishId = SCOPE_IDENTITY();";

			var insertedDishes = await _context.Dishes.FromSqlRaw(sqlInsertDish,
				dish.DishName, dish.Price, dish.Description, dish.Visible, dish.CategoryId).ToListAsync();
			
			var dishInserted = insertedDishes.FirstOrDefault();

			if (dishInserted == null)
			{
				throw new Exception("Failed to insert dish");
			}

			return dishInserted;
		}

		public async Task DeleteAsync(int id)
		{
			string sqlGetDishById = "SELECT * FROM Dishes WHERE DishId = {0}";

			var dish = await _context.Dishes.FromSqlRaw(sqlGetDishById, id).FirstOrDefaultAsync();

			if (dish == null)
			{
				throw new Exception($"Dish with id {id} not found");
			}
			await DeleteAsync(dish);
		}

		public async Task DeleteAsync(Dish dish)
		{
			string sqlDeleteDish = "DELETE FROM Dishes WHERE DishId = {0}";
			await _context.Database.ExecuteSqlRawAsync(sqlDeleteDish, dish.DishId);
		}

		public async Task UpdateAsync(Dish dish)
		{
			string sqlUpdateDish = @"UPDATE Dishes
									SET DishName = {0}, Price = {1}, Description = {2}, Visible = {3}, CategoryId = {4}
									WHERE DishId = {5}";

			await _context.Database.ExecuteSqlRawAsync(sqlUpdateDish,
				dish.DishName, dish.Price, dish.Description, dish.Visible, dish.CategoryId, dish.DishId);
		}

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
