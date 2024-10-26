using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Models.Entities;
using Repositories.Interface;
using System.Security.Claims;
using WebApp.Models;

namespace WebApp.Controllers
{
	[Route("Dashboard/Feedback")]
	[Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Customer)}")]
	public class FeedBackController : Controller
	{
		private readonly IFeedBackRepository _feedBackRepository;
		private readonly IUserRepository _userRepository;

		public FeedBackController(IFeedBackRepository feedBackRepository, IUserRepository userRepository)
		{
			_feedBackRepository = feedBackRepository;
			_userRepository = userRepository;
		}

		public async Task<IActionResult> Index()
		{
			var feedbacks = await _feedBackRepository.GetAllAsync();
			var feedbackList = feedbacks.Select(feedback => new FeedBackViewModel(feedback));
			return View("FeedBackView", feedbackList);
		}

		[HttpGet("Create")]
		[AllowAnonymous]
		public IActionResult Create()
		{
			var feedBackViewModel = GetUserFeedBackViewModel();
			return PartialView("_FeedbackForm", feedBackViewModel);
		}

		[HttpPost("Create")]
		[AllowAnonymous]
		public async Task<IActionResult> Create(FeedBackViewModel feedBackViewModel)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_FeedbackForm", feedBackViewModel);
			}

			try
			{
				var feedBack = new FeedBack
				{
					FullName = feedBackViewModel.FullName,
					Email = feedBackViewModel.Email,
					Phone = feedBackViewModel.Phone,
					Subject = feedBackViewModel.Subject,
					Body = feedBackViewModel.Body,
					CreateAt = DateTime.Now,
					Status = FeedBackStatus.Pending
				};

				await _feedBackRepository.InsertAsync(feedBack);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while creating the feedback. Please try again later.";
				return PartialView("_FeedbackForm", feedBackViewModel);
			}

			TempData["Success"] = "Feedback sent successfully.";
			ModelState.Clear();
			return PartialView("_FeedbackForm");
		}

		[Route("Details/{id}")]
		public async Task<IActionResult> Details(long id)
		{
			var feedback = await _feedBackRepository.GetByIDAsync(id);
			if (feedback == null)
			{
				return NotFound();
			}

			var feedBackViewModel = new FeedBackViewModel(feedback);
			return View("DetailsFeedBackView", feedBackViewModel);
		}

		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(long id)
		{
			var feedback = await _feedBackRepository.GetByIDAsync(id);
			if (feedback == null)
			{
				return NotFound();
			}

			var feedBackViewModel = new FeedBackViewModel(feedback);
			return View("EditFeedBackView", feedBackViewModel);
		}

		[Route("Edit/{id}")]
		[HttpPost]
		public async Task<IActionResult> Edit(FeedBackViewModel feedBackViewModel)
		{
			if (!ModelState.GetValidationState(nameof(feedBackViewModel.Status))
				.Equals(ModelValidationState.Valid) ||
				!ModelState.GetValidationState(nameof(feedBackViewModel.Note))
				.Equals(ModelValidationState.Valid))
			{
				return View("EditFeedBackView", feedBackViewModel);
			}

			try
			{
				var id = feedBackViewModel.FeedbackId ?? throw new KeyNotFoundException();
				var feedbackEntity = await _feedBackRepository.GetByIDAsync(id) ?? throw new KeyNotFoundException();

				feedbackEntity.Status = feedBackViewModel.Status;
				feedbackEntity.Note = feedBackViewModel.Note;

				await _feedBackRepository.UpdateAsync(feedbackEntity);
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while updating the feedback. Please try again later.";
				return View("EditFeedBackView", feedBackViewModel);
			}

			return RedirectToAction("Index");
		}

		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(long id)
		{
			var feedback = await _feedBackRepository.GetByIDAsync(id);
			if (feedback == null)
			{
				return NotFound();
			}

			var feedBackViewModel = new FeedBackViewModel(feedback);
			return View("DeleteFeedBackView", feedBackViewModel);
		}

		[HttpPost]
		[Route("Delete/{FeedbackId}")]
		public async Task<IActionResult> DeleteConfirmed(long FeedbackId)
		{
			try
			{
				await _feedBackRepository.DeleteAsync(FeedbackId);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while deleting feedback. Please try again later.";
				return RedirectToAction("Delete", new { FeedbackId });
			}

			return RedirectToAction("Index");
		}

		private FeedBackViewModel GetUserFeedBackViewModel()
		{
			return new FeedBackViewModel()
			{
				FullName = User.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty,
				Email = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
				Phone = User.FindFirst(ClaimTypes.MobilePhone)?.Value ?? string.Empty
			};
		}
	}
}
