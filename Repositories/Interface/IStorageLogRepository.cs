using Models.Entities;

namespace Repositories.Interface
{
	public interface IStorageLogRepository
	{
		Task<IEnumerable<StorageLog>> GetAllAsync();
		Task<StorageLog?> GetByIDAsync(long id);
		Task InsertAsync(StorageLog storageLog);
		Task DeleteAsync(long id);
		Task DeleteAsync(StorageLog storageLog);
		Task UpdateAsync(StorageLog storageLog);
		Task SaveAsync();
	}
}
