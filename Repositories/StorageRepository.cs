using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return await _context.Storages.AsNoTracking().ToListAsync();
        }

        public async Task<Storage?> GetByIDAsync(int id)
        {
            return await _context.Storages.FindAsync(id);
        }

        public async Task InsertAsync(Storage storage)
        {
            await _context.Storages.AddAsync(storage);
            await SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var storage = await _context.Storages.FindAsync(id);
            if (storage == null)
            {
                throw new Exception($"Storage with id {id} not found");
            }
            await DeleteAsync(storage);
        }

        public async Task DeleteAsync(Storage storage)
        {
            if (_context.Entry(storage).State == EntityState.Detached)
            {
                _context.Storages.Attach(storage);
            }
            _context.Storages.Remove(storage);
            await SaveAsync();
        }

        public async Task UpdateAsync(Storage storage)
        {
            _context.Storages.Attach(storage);
            _context.Entry(storage).State = EntityState.Modified;
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
