using Models.Entities;

namespace Repositories.Interface
{
	public interface IDishCategoryRepository
	{
		Task<IEnumerable<DishCategory>> GetAllAsync();
		Task<DishCategory?> GetByIDAsync(int id);
		Task<DishCategory> InsertAsync(DishCategory dishCategory);
		Task DeleteAsync(int id);
		Task DeleteAsync(DishCategory dishCategory);
		Task UpdateAsync(DishCategory dishCategory);
	}
}
