using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;
using System.Globalization;

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
            string sqlGetAllReservations = "SELECT * FROM Reservations";
            string sqlGetCustomersByIds = "SELECT * FROM Users WHERE UserId IN ({0})";

            var reservations = await _context.Reservations.FromSqlRaw(sqlGetAllReservations).ToListAsync();

            var customerIds = reservations.Select(r => r.CustomerId).Distinct().ToList();
            if (customerIds.Count != 0)
            {
                var customers = await _context.Users
                    .FromSqlRaw(string.Format(sqlGetCustomersByIds, string.Join(",", customerIds)))
                    .ToDictionaryAsync(c => c.UserId);

                foreach (var reservation in reservations)
                {
                    if (customers.TryGetValue(reservation.CustomerId, out var customer))
                    {
                        reservation.Customer = customer;
                    }
                }
            }

            return reservations;
        }

        public async Task<Reservation?> GetByIDAsync(long id)
        {
            string sqlGetReservationById = "SELECT * FROM Reservations WHERE ResId = {0}";
            string sqlGetCustomerById = "SELECT * FROM Users WHERE UserId = {0}";
            string sqlGetOrderByReservationId = "SELECT * FROM Orders WHERE ResId = {0}";

			var reservation = await _context.Reservations.FromSqlRaw(sqlGetReservationById, id).FirstOrDefaultAsync();

            if (reservation == null)
            {
                return null;
            }

			// Include customer
			var customer = await _context.Users
                .FromSqlRaw(sqlGetCustomerById, reservation.CustomerId)
                .FirstOrDefaultAsync();
            if (customer != null)
            {
                reservation.Customer = customer;
            }

			// Include order
			var order = await _context.Orders
				.FromSqlRaw(sqlGetOrderByReservationId, reservation.ResId)
				.FirstOrDefaultAsync();
			if (order != null)
			{
				reservation.Order = order;
			}


			return reservation;
        }

        public async Task<Reservation> InsertAsync(Reservation reservation)
        {
            string sqlInsertReservation = @"INSERT INTO Reservations (CustomerId, PartySize, ResDate, ResTime, Status, DepositAmount, Notes) 
                                    VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6});
                                    SELECT * FROM Reservations WHERE ResId = SCOPE_IDENTITY();";

            var insertedReservations = await _context.Reservations.FromSqlRaw(sqlInsertReservation,
                reservation.CustomerId, reservation.PartySize, reservation.ResDate, reservation.ResTime, reservation.Status, reservation.DepositAmount, reservation.Notes).ToListAsync();

            var reservationInserted = insertedReservations.FirstOrDefault();

            if (reservationInserted == null)
            {
                throw new Exception("Failed to insert reservation");
            }

            return reservationInserted;
        }

        public async Task DeleteAsync(long id)
        {
            string sqlGetReservationById = "SELECT * FROM Reservations WHERE ResId = {0}";

            var reservation = await _context.Reservations.FromSqlRaw(sqlGetReservationById, id).FirstOrDefaultAsync();

            if (reservation == null)
            {
                throw new Exception($"Reservation with id {id} not found");
            }
            await DeleteAsync(reservation);
        }

        public async Task DeleteAsync(Reservation reservation)
        {
            string sqlDeleteReservation = "DELETE FROM Reservations WHERE ResId = {0}";
            await _context.Database.ExecuteSqlRawAsync(string.Format(sqlDeleteReservation, reservation.ResId));
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            string sqlUpdateReservation = @"UPDATE Reservations
                                    SET CustomerId = {0}, PartySize = {1}, ResDate = {2}, ResTime = {3}, Status = {4}, DepositAmount = {5}, Notes = {6}
                                    WHERE ResId = {7}";

            await _context.Database.ExecuteSqlRawAsync(sqlUpdateReservation,
                reservation.CustomerId, reservation.PartySize, reservation.ResDate, reservation.ResTime, reservation.Status, reservation.DepositAmount, reservation.Notes, reservation.ResId);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
