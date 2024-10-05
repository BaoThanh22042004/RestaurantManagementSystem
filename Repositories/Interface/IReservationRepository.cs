using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<Reservation?> GetByIDAsync(long id); 
        Task InsertAsync(Reservation reservation);
        Task DeleteAsync(long id); 
        Task DeleteAsync(Reservation reservation);
        Task UpdateAsync(Reservation reservation);
        Task SaveAsync();
    }
}
