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
			string sqlGetAllTables = "SELECT * FROM Tables";
			var tables = await _context.Tables.FromSqlRaw(sqlGetAllTables).ToListAsync();
			return tables;
		}

		public async Task<Table?> GetByIDAsync(int id)
		{
			string sqlGetTableById = "SELECT * FROM Tables WHERE TableId = {0}";
			var table = await _context.Tables.FromSqlRaw(sqlGetTableById, id).FirstOrDefaultAsync();
			return table;
		}

		public async Task<Table> InsertAsync(Table table)
		{
			string sqlInsertTable = @"INSERT INTO Tables ([TableName] ,[Capacity] ,[Status] ,[ResTime] ,[Notes])
											VALUES ({0}, {1}, {2}, {3}, {4});
											SELECT * FROM Tables WHERE TableId = SCOPE_IDENTITY();";

			var insertedTables = await _context.Tables.FromSqlRaw(sqlInsertTable, table.TableName, table.Capacity, table.Status, table.ResTime, table.Notes).FirstOrDefaultAsync();
			if (insertedTables == null)
			{
				throw new Exception("Failed to insert table");
			}
			return insertedTables;
		}

		public async Task DeleteAsync(int id)
		{
			string sqlGetTableById = "SELECT * FROM Tables WHERE TableId = {0}";

			var table = await _context.Tables.FromSqlRaw(sqlGetTableById, id).FirstOrDefaultAsync();
			if (table == null)
			{
				throw new Exception($"Table with id {id} not found");
			}

			await DeleteAsync(table);
		}

		public async Task DeleteAsync(Table table)
		{
			string sqlDeleteTable = "DELETE FROM Tables WHERE TableId = {0}";
			await _context.Database.ExecuteSqlRawAsync(sqlDeleteTable, table.TableId);
		}

		public async Task UpdateAsync(Table table)
		{
			string sqlUpdateTable = @"UPDATE Tables SET TableName = {0}, Capacity = {1}, Status = {2}, ResTime = {3}, Notes = {4} WHERE TableId = {5}";
			await _context.Database.ExecuteSqlRawAsync(sqlUpdateTable, table.TableName, table.Capacity, table.Status, table.ResTime, table.Notes, table.TableId);
		}+
		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
