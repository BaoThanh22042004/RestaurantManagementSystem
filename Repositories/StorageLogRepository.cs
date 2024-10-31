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
            string sqlGetAllStorageLogs = "SELECT * FROM StorageLogs";
            string sqlGetStoragesByIds = "SELECT * FROM Storages WHERE ItemId IN ({0})";

            var storageLogs = await _context.StorageLogs.FromSqlRaw(sqlGetAllStorageLogs).ToListAsync();

            var storageIds = storageLogs.Select(log => log.ItemId).Distinct().ToList();

            var storages = await _context.Storages
                .FromSqlRaw(string.Format(sqlGetStoragesByIds, string.Join(",", storageIds)))
                .ToDictionaryAsync(s => s.ItemId);

            foreach (var storageLog in storageLogs)
            {
                if (storages.TryGetValue(storageLog.ItemId, out var storage))
                {
                    storageLog.StorageItem = storage;
                }
            }

            return storageLogs;
        }


        public async Task<StorageLog?> GetByIDAsync(long id)
        {
            string sqlGetStorageLogById = "SELECT * FROM StorageLogs WHERE LogId = {0}";
            string sqlGetStorageById = "SELECT * FROM Storages WHERE ItemId = {0}";
            string sqlGetUserById = "SELECT * FROM Users WHERE UserId = {0}";

			var storageLog = await _context.StorageLogs.FromSqlRaw(sqlGetStorageLogById, id).FirstOrDefaultAsync();

            if (storageLog == null)
            {
                return null;
            }

			// Include storage item
			var storage = await _context.Storages
                .FromSqlRaw(sqlGetStorageById, storageLog.ItemId)
                .FirstOrDefaultAsync();

            if (storage != null)
            {
                storageLog.StorageItem = storage;
            }

			// Include creator
            var user = await _context.Users
				.FromSqlRaw(sqlGetUserById, storageLog.CreatedBy)
				.FirstOrDefaultAsync();
			
            if (user != null)
			{
				storageLog.Creator = user;
			}

			return storageLog;
        }


        public async Task<StorageLog> InsertAsync(StorageLog storageLog)
        {
            string sqlGetStorageLogById = "SELECT * FROM StorageLogs WHERE LogId = {0}";
            string sqlInsertStorageLog = @"INSERT INTO StorageLogs ([CreatedBy],[CreatedAt],[ItemId],[ChangeQuantity],[RemainQuantity],[Action],[Cost],[Description])
                                           VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7});
                                           SELECT * FROM StorageLogs WHERE LogId = SCOPE_IDENTITY();";

            var insertedStorageLogs = await _context.StorageLogs.FromSqlRaw(sqlInsertStorageLog,
                storageLog.CreatedBy, storageLog.CreatedAt, storageLog.ItemId, storageLog.ChangeQuantity, storageLog.RemainQuantity, storageLog.Action, storageLog.Cost, storageLog.Description).ToListAsync();

            var insertedStorageLog = insertedStorageLogs.FirstOrDefault();

            if (insertedStorageLog == null)
            {
                throw new Exception("Insert storage log failed");
            }
            return insertedStorageLog;
        }

        public async Task DeleteAsync(long id)
        {
            string sqlGetStorageLogById = "SELECT * FROM StorageLogs WHERE LogId = {0}";

            var storageLog = await _context.StorageLogs.FromSqlRaw(sqlGetStorageLogById, id).FirstOrDefaultAsync();

            if (storageLog == null)
            {
                throw new Exception($"Storage log with id {id} not found");
            }
            await DeleteAsync(storageLog);
        }

        public async Task DeleteAsync(StorageLog storageLog)
        {
            string sqlDeleteStorageLog = "DELETE FROM StorageLogs WHERE LogId = {0}";
            await _context.Database.ExecuteSqlRawAsync(sqlDeleteStorageLog, storageLog.LogId);
        }

        public async Task UpdateAsync(StorageLog storageLog)
        {
            string sqlUpdateStorageLog = "UPDATE StorageLogs SET CreatedBy = {0}, CreatedAt = {1}, ItemId = {2}, ChangeQuantity = {3}, RemainQuantity = {4}, Action = {5}, Cost = {6}, Description = {7} WHERE LogId = {8}";
            await _context.Database.ExecuteSqlRawAsync(sqlUpdateStorageLog,
                storageLog.CreatedBy, storageLog.CreatedAt, storageLog.ItemId, storageLog.ChangeQuantity, storageLog.RemainQuantity, storageLog.Action, storageLog.Cost, storageLog.Description, storageLog.LogId);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

		public async Task<IEnumerable<StorageLog>> GetAllByItemIdAsync(int id)
		{
			string sqlGetAllStorageLogsByItemId = "SELECT * FROM StorageLogs WHERE ItemId = {0}";
			return await _context.StorageLogs.FromSqlRaw(sqlGetAllStorageLogsByItemId, id).ToListAsync();
		}
	}
}
