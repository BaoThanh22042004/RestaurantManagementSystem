﻿using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
	public class ReservationRepository : IReservationRepository, IDisposable
    {
        private readonly DBContext _context;

        public ReservationRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations
                .Include(r => r.Customer)
                .AsNoTracking().ToListAsync();
        }

        public async Task<Reservation?> GetByIDAsync(long id)
        {
            return await _context.Reservations
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.ResId == id);
        }

        public async Task<Reservation> InsertAsync(Reservation reservation)
        {
            var entityEntry = await _context.Reservations.AddAsync(reservation);
            await SaveAsync();
            return entityEntry.Entity;
		}

        public async Task DeleteAsync(long id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                throw new Exception($"Reservation with id {id} not found");
            }
            await DeleteAsync(reservation);
        }

        public async Task DeleteAsync(Reservation reservation)
        {
            if (_context.Entry(reservation).State == EntityState.Detached)
            {
                _context.Reservations.Attach(reservation);
            }
            _context.Reservations.Remove(reservation);
            await SaveAsync();
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Attach(reservation);
            _context.Entry(reservation).State = EntityState.Modified;
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
