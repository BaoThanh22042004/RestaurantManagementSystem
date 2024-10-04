using Models.Entities;

namespace Repositories.Interface
{
	public interface IUserRepository
	{
		Task<IEnumerable<User>> GetAllAsync();
		Task<User?> GetByIDAsync(int id);
		Task InsertAsync(User user);
		Task DeleteAsync(int id);
		Task DeleteAsync(User user);
		Task UpdateAsync(User user);
		Task SaveAsync();
	}
}
