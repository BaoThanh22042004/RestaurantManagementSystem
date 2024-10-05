using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
    public class BillRepository : IBillRepository, IDisposable
    {
        private readonly DBContext _context;

        public BillRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bill>> GetAllAsync()
        {
            return await _context.Bills.AsNoTracking().ToListAsync();
        }

        public async Task<Bill?> GetByIDAsync(long id)
        {
            return await _context.Bills.FindAsync(id);
        }

        public async Task InsertAsync(Bill bill)
        {
            await _context.Bills.AddAsync(bill);
            await SaveAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                throw new Exception($"Bill with id {id} not found");
            }
            await DeleteAsync(bill);
        }

        public async Task DeleteAsync(Bill bill)
        {
            if (_context.Entry(bill).State == EntityState.Detached)
            {
                _context.Bills.Attach(bill);
            }
            _context.Bills.Remove(bill);
            await SaveAsync();
        }

        public async Task UpdateAsync(Bill bill)
        {
            _context.Bills.Attach(bill);
            _context.Entry(bill).State = EntityState.Modified;
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
