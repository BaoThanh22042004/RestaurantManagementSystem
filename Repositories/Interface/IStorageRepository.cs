using Models.Entities;

namespace Repositories.Interface
{
	public interface IStorageRepository
	{
		Task<IEnumerable<Storage>> GetAllAsync();
		Task<Storage?> GetByIDAsync(int id);
		Task InsertAsync(Storage storage);
		Task DeleteAsync(int id);
		Task DeleteAsync(Storage storage);
		Task UpdateAsync(Storage storage);
		Task SaveAsync();
	}
}
