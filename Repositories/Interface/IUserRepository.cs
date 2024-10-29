using Models.Entities;

namespace Repositories.Interface
{
	public interface IUserRepository
	{
		Task<IEnumerable<User>> GetAllAsync();
		Task<User?> GetByIDAsync(int id);
		Task<User> InsertAsync(User user);
		Task DeleteAsync(int id);
		Task DeleteAsync(User user);
		Task UpdateAsync(User user, string? password = null);
		Task<User?> ValidateLoginAsync(string username, string password);
		Task<User?> GetByUsernameAsync(string username);
	}
}
