using Models.Entities;

namespace Repositories.Interface
{
	public interface IOrderItemRepository
	{
		Task<IEnumerable<OrderItem>> GetAllAsync();
		Task<OrderItem?> GetByIDAsync(long id);
		Task<OrderItem> InsertAsync(OrderItem orderItem);
		Task DeleteAsync(long id);
		Task DeleteAsync(OrderItem orderItem);
		Task UpdateAsync(OrderItem orderItem);
        Task<IEnumerable<OrderItem>?> GetAllByOrderIdAsync(long orderId);

    }
}
