using Models.Entities;

namespace Repositories.Interface
{
	public interface IOrderRepository
	{
		Task<IEnumerable<Order>> GetAllAsync();
		Task<IEnumerable<Order>> GetAllIn24Hours();
		Task<Order?> GetByIDAsync(long id);
		Task<Order> InsertAsync(Order order);
		Task DeleteAsync(long id);
		Task DeleteAsync(Order order);
		Task UpdateAsync(Order order);
	}
}
