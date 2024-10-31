using Models.Entities;

namespace Repositories.Interface
{
	public interface IReservationRepository
	{
		Task<IEnumerable<Reservation>> GetAllAsync();
		Task<Reservation?> GetByIDAsync(long id);
		Task<Reservation> InsertAsync(Reservation reservation);
		Task DeleteAsync(long id);
		Task DeleteAsync(Reservation reservation);
		Task UpdateAsync(Reservation reservation);
	}
}
