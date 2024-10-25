using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using System.Security.Claims;
using WebApp.Models;
using Microsoft.EntityFrameworkCore; 

namespace WebApp.Controllers
{
    [Route("Dashboard/Feedback")]
    [Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Waitstaff)}")]
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

        [Route("Create")]
        public IActionResult Create()
        {
            return View("CreateFeedBackView");
        }

        [Route("Create")]
        [HttpPost]
        public async Task<IActionResult> Create(FeedBackViewModel feedBackViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateFeedBackView", feedBackViewModel);
            }

            try
            {
                var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                var phoneClaim = User.FindFirst("Phone")?.Value;

                if (customerIdClaim == null || emailClaim == null || phoneClaim == null)
                {
                    TempData["Error"] = "User information is missing. Please log in again.";
                    return View("CreateFeedBackView", feedBackViewModel);
                }

               
                var feedBack = new FeedBack
                {
                    CustomerId = int.Parse(customerIdClaim), 
                    Email = emailClaim, 
                    Phone = phoneClaim,
                    CreateAt = DateTime.Now,
                    Subject = feedBackViewModel.Subject,
                    Body = feedBackViewModel.Body,
                    Status = FeedBackStatus.Pending
                };

                await _feedBackRepository.InsertAsync(feedBack);
            }
            catch (Exception e)
            {
                TempData["Error"] = "An error occurred while creating the feedback. Please try again later.";
                return View("CreateFeedBackView", feedBackViewModel);
            }

            return RedirectToAction("Index");
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
            if (!ModelState.IsValid)
            {
                return View("EditFeedBackView", feedBackViewModel);
            }

            try
            {
                var id = feedBackViewModel.FeedbackId ?? throw new KeyNotFoundException();
                var feedbackEntity = await _feedBackRepository.GetByIDAsync(id);
                if (feedbackEntity == null)
                {
                    throw new KeyNotFoundException();
                }

                // Lấy thông tin từ Claims của người dùng đang đăng nhập
                var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
                var phoneClaim = User.FindFirst("Phone")?.Value;

                if (customerIdClaim == null || emailClaim == null || phoneClaim == null)
                {
                    TempData["Error"] = "User information is missing. Please log in again.";
                    return View("EditFeedBackView", feedBackViewModel);
                }

              
                feedbackEntity.CustomerId = int.Parse(customerIdClaim); 
                feedbackEntity.Email = emailClaim; 
                feedbackEntity.Phone = phoneClaim; 
                feedbackEntity.CreateAt = feedBackViewModel.CreateAt;
                feedbackEntity.Subject = feedBackViewModel.Subject;
                feedbackEntity.Body = feedBackViewModel.Body;
                feedbackEntity.Status = (FeedBackStatus)feedBackViewModel.Status;

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
    }
}
