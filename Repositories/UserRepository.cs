using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;
using System.Security.Cryptography;
using System.Text;

namespace Repositories
{
	public class UserRepository : IUserRepository, IDisposable
	{
		private readonly DBContext _context;

		public UserRepository(DBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<User>> GetAllAsync()
		{
			return await _context.Users.AsNoTracking().ToListAsync();
		}

		public async Task<User?> GetByIDAsync(int id)
		{
			return await _context.Users.FindAsync(id);
		}

		public async Task InsertAsync(User user)
		{
			user.Password = HashPassword(user.Password);
			await _context.Users.AddAsync(user);
			await SaveAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var user = await _context.Users.FindAsync(id);
			if (user == null)
			{
				throw new Exception($"User with id {id} not found");
			}
			await DeleteAsync(user);
		}

		public async Task DeleteAsync(User user)
		{
			if (_context.Entry(user).State == EntityState.Detached)
			{
				_context.Users.Attach(user);
			}
			_context.Users.Remove(user);
			await SaveAsync();
		}

		public async Task UpdateAsync(User user)
		{
			_context.Users.Attach(user);
			_context.Entry(user).State = EntityState.Modified;
			await SaveAsync();
		}

		public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task<User?> ValidateLoginAsync(string username, string password)
		{
			var user = await _context.Users.AsNoTracking().Where(user => user.IsActive).SingleOrDefaultAsync(user => user.Username == username);
			if (user == null || !VerifyPassword(password, user.Password))
			{
				return null;
			}

			return user;
		}

		private static string HashPassword(string password)
		{
			byte[] inputBytes = Encoding.UTF8.GetBytes(password);
			byte[] hashBytes = MD5.HashData(inputBytes);

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hashBytes.Length; i++)
			{
				sb.Append(hashBytes[i].ToString("x2"));
			}

			return sb.ToString();
		}

		private static bool VerifyPassword(string password, string hashPassword)
		{
			return HashPassword(password).Equals(hashPassword, StringComparison.OrdinalIgnoreCase);
		}

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
