using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
	public class StorageLogRepository : IStorageLogRepository, IDisposable
	{
		private readonly DBContext _context;

		public StorageLogRepository(DBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<StorageLog>> GetAllAsync()
		{
			return await _context.StorageLogs.AsNoTracking().ToListAsync();
		}

		public async Task<StorageLog?> GetByIDAsync(long id)
		{
			return await _context.StorageLogs.FindAsync(id);
		}

		public async Task InsertAsync(StorageLog storageLog)
		{
			await _context.StorageLogs.AddAsync(storageLog);
			await SaveAsync();
		}

		public async Task DeleteAsync(long id)
		{
			var storageLog = await _context.StorageLogs.FindAsync(id);
			if (storageLog == null)
			{
				throw new Exception($"Storage Log with id {id} not found");
			}
			await DeleteAsync(storageLog);
		}

		public async Task DeleteAsync(StorageLog storageLog)
		{
			if (_context.Entry(storageLog).State == EntityState.Detached)
			{
				_context.StorageLogs.Attach(storageLog);
			}
			_context.StorageLogs.Remove(storageLog);
			await SaveAsync();
		}

		public async Task UpdateAsync(StorageLog storageLog)
		{
			_context.StorageLogs.Attach(storageLog);
			_context.Entry(storageLog).State = EntityState.Modified;
			await SaveAsync();
		}

		public async Task SaveAsync()
		{
			await _context.SaveChangesAsync();
		}

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
