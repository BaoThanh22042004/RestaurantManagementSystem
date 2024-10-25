using Microsoft.EntityFrameworkCore;
using Models;
using Models.Entities;
using Repositories.Interface;

namespace Repositories
{
    public class FeedBackRepository : IFeedBackRepository, IDisposable
    {
        private readonly DBContext _context;

        public FeedBackRepository(DBContext context)
        {
            _context = context;
        }



        public async Task<IEnumerable<FeedBack>> GetAllAsync()
        {
            return await _context.Feedbacks.Include(FeedBack => FeedBack.Customer)
                                        .AsNoTracking().ToListAsync();
        }

        public async Task<FeedBack?> GetByIDAsync(long id)
        {
            return await _context.Feedbacks
                .Include(FeedBack => FeedBack.Customer)
                .FirstOrDefaultAsync(FeedBack => FeedBack.FeedbackId.Equals(id));
        }

        public async Task InsertAsync(FeedBack feedBack)
        {
            var entityEntry = await _context.Feedbacks.AddAsync(feedBack);
            await SaveAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var feedBack = await _context.Feedbacks.FindAsync(id);
            if (feedBack == null)
            {
                throw new Exception($"Feedback with id {id} not found");
            }
            await DeleteAsync(feedBack);
        }

        public async Task DeleteAsync(FeedBack feedBack)
        {
            if (_context.Entry(feedBack).State == EntityState.Detached)
            {
                _context.Feedbacks.Attach(feedBack);
            }
            _context.Feedbacks.Remove(feedBack);
            await SaveAsync();
        }

        public async Task UpdateAsync(FeedBack feedBack)
        {
            _context.Feedbacks.Attach(feedBack);
            _context.Entry(feedBack).State = EntityState.Modified;
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
