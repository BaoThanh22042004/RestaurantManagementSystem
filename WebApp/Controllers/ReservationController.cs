using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("Dashboard/Reservation")]
    [Authorize(Roles = $"{nameof(Role.Manager)}")]
    public class ReservationController : Controller
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IUserRepository _userRepository;

        public ReservationController(IReservationRepository reservationRepository, IUserRepository userRepository)
        {
            _reservationRepository = reservationRepository;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var reservations = await _reservationRepository.GetAllAsync();
            var reservationList = reservations.Select(reservation => new ReservationViewModel(reservation));
            return View("ReservationView", reservationList);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var reservationViewModel = new ReservationViewModel()
            {
                Customers = await GetCustomerList(),
            };
            return PartialView("_CreateReservationModal", reservationViewModel);
        }

        private async Task<IEnumerable<User>> GetCustomerList()
        {
            return (await _userRepository.GetAllAsync()).Where(c => c.Role == Role.Customer);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(ReservationViewModel reservationViewModel)
        {
            if (!ModelState.IsValid)
            {
                reservationViewModel.Customers = await GetCustomerList();
                return PartialView("_CreateReservationModal", reservationViewModel);
            }

            try
            {
                var reservation = new Reservation
                {
                    CustomerId = reservationViewModel.CustomerId,
                    PartySize = reservationViewModel.PartySize,
                    ResDate = reservationViewModel.ResDate,
                    ResTime = reservationViewModel.ResTime,
                    Status = reservationViewModel.Status,
                    DepositAmount = reservationViewModel.DepositAmount,
                    Notes = reservationViewModel.Notes,
                };

                await _reservationRepository.InsertAsync(reservation);
            }
            catch (ArgumentException e)
            {
                TempData["Error"] = e.Message;
                return PartialView("_CreateReservationModal", reservationViewModel);
            }
            return Json(new { success = true });
        }

        [Route("Details/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            var reservation = await _reservationRepository.GetByIDAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var reservationViewModel = new ReservationViewModel(reservation)
            {
                Customers = await GetCustomerList()
            };
            return PartialView("_DetailsReservationModal", reservationViewModel);
        }

        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(long id)
        {
            var reservation = await _reservationRepository.GetByIDAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var reservationViewModel = new ReservationViewModel(reservation)
            {
                Customers = await GetCustomerList()
            };
            return PartialView("_EditReservationModal", reservationViewModel);
        }

        [HttpPost]
        [Route("Edit/{ResId}")]
        public async Task<IActionResult> Edit(ReservationViewModel reservationViewModel, long ResId)
        {
            if (!ModelState.IsValid)
            {
                reservationViewModel.Customers = await GetCustomerList();
                return PartialView("_EditReservationModal", reservationViewModel);
            }

            try
            {
                var reservationEntity = await _reservationRepository.GetByIDAsync(ResId);
                if (reservationEntity == null)
                {
                    return NotFound();
                }

                reservationEntity.CustomerId = reservationViewModel.CustomerId;
                reservationEntity.PartySize = reservationViewModel.PartySize;
                reservationEntity.ResDate = reservationViewModel.ResDate;
                reservationEntity.ResTime = reservationViewModel.ResTime;
                reservationEntity.Status = reservationViewModel.Status;
                reservationEntity.DepositAmount = reservationViewModel.DepositAmount;
                reservationEntity.Notes = reservationViewModel.Notes;

                await _reservationRepository.UpdateAsync(reservationEntity);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            return Json(new { success = true });
        }

        [Route("Reservation/Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var reservation = await _reservationRepository.GetByIDAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var reservationViewModel = new ReservationViewModel(reservation) { Customers = await GetCustomerList() };
            return PartialView("_DeleteReservationModal", reservationViewModel);
        }

        [HttpPost]
        [Route("Reservation/Delete/{ResId}")]
        public async Task<IActionResult> DeleteConfirmed(long ResId)
        {
            try
            {
                await _reservationRepository.DeleteAsync(ResId);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the reservation. Please try again later.";
                return RedirectToAction("Delete", new { id = ResId });
            }

            return Json(new { success = true });
        }
    }
}
