using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;

namespace Repositories
{
	public class UserRepository : GenericRepository<User>
	{
		public UserRepository(DBContext context) : base(context)
		{
		}

		public async Task<User?> ValidateLogin(string username, string password)
		{
			var user = await dbSet.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
			if (user == null || !VerifyPassword(password, user.Password))
			{
				return null;
			}
			return user;
		}

		private static string HashPassword(string password)
		{
			// TODO: Implement password hashing
			return string.Empty;
		}

		private static bool VerifyPassword(string password, string hash)
		{
			// TODO: Implement password verification
			return false;
		}
	}
}
