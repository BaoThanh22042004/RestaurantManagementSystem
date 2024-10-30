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
			string sqlGetAllFeedbacks = "SELECT * FROM Feedbacks";
			return await _context.Feedbacks.FromSqlRaw(sqlGetAllFeedbacks).ToListAsync();
		}

		public async Task<FeedBack?> GetByIDAsync(long id)
		{
			string sqlGetFeedbackById = "SELECT * FROM Feedbacks WHERE FeedbackId = {0}";
			return await _context.Feedbacks.FromSqlRaw(sqlGetFeedbackById, id).FirstOrDefaultAsync();
		}

		public async Task<FeedBack> InsertAsync(FeedBack feedBack)
		{
			string sqlInsertFeedback = @"INSERT INTO Feedbacks (FullName, Email, Phone, Subject, Body, CreateAt, Status, Note)
										 VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})
										 SELECT * FROM Feedbacks WHERE FeedbackId = SCOPE_IDENTITY()";

			var insertedFeedback = await _context.Feedbacks.FromSqlRaw(sqlInsertFeedback,
																		feedBack.FullName, feedBack.Email, feedBack.Phone,
																		feedBack.Subject, feedBack.Body, feedBack.CreateAt,
																		feedBack.Status, feedBack.Note).FirstOrDefaultAsync();

			if (insertedFeedback == null)
			{
				throw new Exception("Failed to insert feedback");
			}

			return insertedFeedback;
		}

		public async Task DeleteAsync(long id)
		{
			string sqlGetFeedbackById = "SELECT * FROM Feedbacks WHERE FeedbackId = {0}";

			var feedback = await _context.Feedbacks.FromSqlRaw(sqlGetFeedbackById, id).FirstOrDefaultAsync();
			if (feedback == null) {
				throw new Exception($"Feedback with id {id} not found");
			}
			await DeleteAsync(feedback);
		}

		public async Task DeleteAsync(FeedBack feedBack)
		{
			string sqlDeleteFeedback = "DELETE FROM Feedbacks WHERE FeedbackId = {0}";
			await _context.Database.ExecuteSqlRawAsync(sqlDeleteFeedback, feedBack.FeedbackId);
		}

		public async Task UpdateAsync(FeedBack feedBack)
		{
			string sqlUpdateFeedback = @"UPDATE Feedbacks
										 SET FullName = {0}, Email = {1}, Phone = {2}, Subject = {3}, Body = {4}, CreateAt = {5}, Status = {6}, Note = {7}
										 WHERE FeedbackId = {8}";

			await _context.Database.ExecuteSqlRawAsync(sqlUpdateFeedback,
														feedBack.FullName, feedBack.Email, feedBack.Phone,
														feedBack.Subject, feedBack.Body, feedBack.CreateAt,
														feedBack.Status, feedBack.Note, feedBack.FeedbackId);
		}

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
