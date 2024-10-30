using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
    public class StorageRepository : IStorageRepository, IDisposable
    {
        private readonly DBContext _context;

        public StorageRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Storage>> GetAllAsync()
        {
            string sqlGetAllStorages = "SELECT * FROM Storages";
            return await _context.Storages.FromSqlRaw(sqlGetAllStorages).ToListAsync();

        }

        public async Task<Storage?> GetByIDAsync(int id)
        {
            string sqlGetStorageById = "SELECT * FROM Storages WHERE ItemId = {0}";
            return await _context.Storages.FromSqlRaw(sqlGetStorageById, id).FirstOrDefaultAsync();
        }

        public async Task<Storage> InsertAsync(Storage storage)
        {
            string sqlGetStorageByItemName = "SELECT * FROM Storages WHERE ItemName = {0}";
            string sqlInsertStorage = @"INSERT INTO Storages ([ItemName],[Unit],[Quantity]) VALUES ({0}, {1}, {2});
                                        SELECT * FROM Storages WHERE ItemId = SCOPE_IDENTITY();";
            var isContainStorage = await _context.Storages.FromSqlRaw(sqlGetStorageByItemName, storage.ItemName).AnyAsync();
            if (isContainStorage)
            {
                throw new ArgumentException($"Storage {storage.ItemName} is already exist");
            }
            var insertedStorages = await _context.Storages.FromSqlRaw(sqlInsertStorage, storage.ItemName, storage.Unit, storage.Quantity).ToListAsync();
            var storageInserted = insertedStorages.FirstOrDefault();

            if (storageInserted == null)
            {
                throw new Exception("Failed to insert storage");
            }
            return storageInserted;
        }
        public async Task DeleteAsync(int id)
        {
            string sqlGetStorageById = "SELECT * FROM Storages WHERE ItemId = {0}";

            var storage = await _context.Storages.FromSqlRaw(sqlGetStorageById, id).FirstOrDefaultAsync();
            if (storage == null)
            {
                throw new ArgumentException($"Storage with id {id} not found");
            }
            await DeleteAsync(storage);
        }
        public async Task DeleteAsync(Storage storage)
        {
            string sqlDeleteStorage = "DELETE FROM Storages WHERE ItemId = {0}";
            await _context.Database.ExecuteSqlRawAsync(sqlDeleteStorage, storage.ItemId);
        }

        public async Task UpdateAsync(Storage storage)
        {
            string sqlUpdateStorage = "UPDATE Storages SET ItemName = {0}, Unit = {1} WHERE ItemId = {2}";
            await _context.Database.ExecuteSqlRawAsync(sqlUpdateStorage, storage.ItemName, storage.Unit, storage.ItemId);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
