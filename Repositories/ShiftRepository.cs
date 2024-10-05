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
    public class ShiftRepository : IShiftRepository, IDisposable
    {

        private readonly DBContext _context;

        public ShiftRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Shift>> GetAllAsync()
        {
            return await _context.Shifts.AsNoTracking().ToListAsync();
        }

        public async Task<Shift?> GetByIDAsync(int id)
        {
            return await _context.Shifts.FindAsync(id);
        }

        public async Task InsertAsync(Shift shift)
        {
            await _context.Shifts.AddAsync(shift);
            await SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null)
            {
                throw new Exception($"Shift with id {id} not found");
            }
            await DeleteAsync(shift);
        }

        public async Task DeleteAsync(Shift shift)
        {
            if (_context.Entry(shift).State == EntityState.Detached)
            {
                _context.Shifts.Attach(shift);
            }
            _context.Shifts.Remove(shift);
            await SaveAsync();
        }

        public async Task UpdateAsync(Shift shift)
        {
            _context.Shifts.Attach(shift);
            _context.Entry(shift).State = EntityState.Modified;
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
