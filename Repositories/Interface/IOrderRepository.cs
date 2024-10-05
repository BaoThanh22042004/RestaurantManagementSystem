using Models.Entities;

namespace Repositories.Interface
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIDAsync(long id);
        Task InsertAsync(Order order);
        Task DeleteAsync(long id);
        Task DeleteAsync(Order order);
        Task UpdateAsync(Order order);
        Task SaveAsync();
    }
}
