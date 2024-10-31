using Models.Entities;

namespace Repositories.Interface
{
	public interface IFeedBackRepository
	{
		Task<IEnumerable<FeedBack>> GetAllAsync();
		Task<FeedBack?> GetByIDAsync(long id);
		Task<FeedBack> InsertAsync(FeedBack feedBack);
		Task DeleteAsync(long id);
		Task DeleteAsync(FeedBack feedBack);
		Task UpdateAsync(FeedBack feedBack);
	}
}
