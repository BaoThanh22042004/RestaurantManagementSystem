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
			string sqlGetAllUsers = "SELECT * FROM Users";
			return await _context.Users.FromSqlRaw(sqlGetAllUsers).ToListAsync();
		}

		public async Task<User?> GetByIDAsync(int id)
		{
			string sqlGetUserById = "SELECT * FROM Users WHERE UserId = {0}";
			return await _context.Users.FromSqlRaw(sqlGetUserById, id).FirstOrDefaultAsync();
		}

		public async Task<User> InsertAsync(User user)
		{
			string sqlGetUserByUsername = "SELECT * FROM Users WHERE Username = {0}";
			string sqlInsertUser = @"INSERT INTO Users (Username, Password, FullName, Email, Phone, Role, Salary, IsActive)
								 VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7});
								 SELECT * FROM Users WHERE UserId = SCOPE_IDENTITY();";

			var isContainUsername = await _context.Users.FromSqlRaw(sqlGetUserByUsername, user.Username).AnyAsync();
			if (isContainUsername)
			{
				throw new ArgumentException($"Username {user.Username} is already taken");
			}
			user.Password = HashPassword(user.Password);
			var insertedUsers = await _context.Users.FromSqlRaw(sqlInsertUser,
				user.Username, user.Password, user.FullName, user.Email, user.Phone, user.Role, user.Salary, user.IsActive).ToListAsync();
			var userInserted = insertedUsers.FirstOrDefault();

			if (userInserted == null)
			{
				throw new Exception("Failed to insert user");
			}

			return userInserted;
		}

		public async Task DeleteAsync(int id)
		{
			string sqlGetUserById = "SELECT * FROM Users WHERE UserId = {0}";

			var user = await _context.Users.FromSqlRaw(sqlGetUserById, id).FirstOrDefaultAsync();
			if (user == null)
			{
				throw new Exception($"User with id {id} not found");
			}
			await DeleteAsync(user);
		}

		public async Task DeleteAsync(User user)
		{
			string sqlDeleteUser = "DELETE FROM Users WHERE UserId = {0}";
			await _context.Database.ExecuteSqlRawAsync(sqlDeleteUser, user.UserId);
		}

		public async Task UpdateAsync(User user, string? password = null)
		{
			string sqlUpdateUserDetails = @"UPDATE Users
										   SET FullName = {0},
											   Email = {1},
											   Phone = {2},
											   Role = {3},
											   Salary = {4},
											   IsActive = {5}
										   WHERE UserId = {6}";
			string sqlUpdateUserPassword = @"UPDATE Users
											SET Password = {0}
											WHERE UserId = {1}";

			if (password != null)
			{
				await _context.Database.ExecuteSqlRawAsync(sqlUpdateUserPassword, HashPassword(password), user.UserId);
			}

			_context.Database.ExecuteSqlRaw(sqlUpdateUserDetails, user.FullName, user.Email, user.Phone, user.Role, user.Salary, user.IsActive, user.UserId);
		}

		public async Task<User?> ValidateLoginAsync(string username, string password)
		{
			string sqlGetActiveUserByUsername = "SELECT * FROM Users WHERE Username = {0} AND IsActive = 1";

			var user = await _context.Users.FromSqlRaw(sqlGetActiveUserByUsername, username).FirstOrDefaultAsync();
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

		public async Task<User?> GetByUsernameAsync(string username)
		{
			string sqlGetUserByUsername = "SELECT * FROM Users WHERE Username = {0}";

			var user = await _context.Users.FromSqlRaw(sqlGetUserByUsername, username).FirstOrDefaultAsync();
			return user;
		}

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
