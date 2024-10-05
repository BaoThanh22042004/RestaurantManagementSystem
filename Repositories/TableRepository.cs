using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
	public class TableRepository : ITableRepository, IDisposable
	{
		private readonly DBContext _context;

		public TableRepository(DBContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Table>> GetAllAsync()
		{
			return await _context.Tables.AsNoTracking().ToListAsync();
		}

		public async Task<Table?> GetByIDAsync(int id)
		{
			return await _context.Tables.FindAsync(id);
		}

		public async Task InsertAsync(Table table)
		{
			await _context.Tables.AddAsync(table);
			await SaveAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var table = await _context.Tables.FindAsync(id);
			if (table == null)
			{
				throw new Exception($"Table with id {id} not found");
			}
			await DeleteAsync(table);
		}

		public async Task DeleteAsync(Table table)
		{
			if (_context.Entry(table).State == EntityState.Detached)
			{
				_context.Tables.Attach(table);
			}
			_context.Tables.Remove(table);
			await SaveAsync();
		}

		public async Task UpdateAsync(Table table)
		{
			_context.Tables.Attach(table);
			_context.Entry(table).State = EntityState.Modified;
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
