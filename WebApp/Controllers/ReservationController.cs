using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Utilities;

namespace WebApp.Controllers
{
    [Route("Reservation")]
    [Authorize(Roles = $"{nameof(Role.Manager)}")]
    public class ReservationController : Controller
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IUserRepository _userRepository;
        private readonly CartManager _cartManager;

		public ReservationController(IReservationRepository reservationRepository, IUserRepository userRepository, CartManager cartManager)
        {
            _reservationRepository = reservationRepository;
            _userRepository = userRepository;
			_cartManager = cartManager;
		}

		public IActionResult Index()
        {
            var cart = _cartManager.GetCartFromCookie(Request);
            var makeReservation = new MakeReservationViewModel
			{
				Cart = cart,
                ResDate = DateOnly.FromDateTime(DateTime.Now),
                ResTime = TimeOnly.FromDateTime(DateTime.Now.AddHours(1))
			};
			return View("MakeReservationView", makeReservation);
        }

		[HttpGet("List")]
		public async Task<IActionResult> List()
        {
            var reservations = await _reservationRepository.GetAllAsync();
            var reservationList = reservations.Select(reservation => new ReservationViewModel(reservation));
            return View("ReservationView", reservationList);
        }

        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var reservationViewModel = new ReservationViewModel()
            {
                Customers = await GetCustomerList(),
            };
            return View("CreateReservationView", reservationViewModel);
        }

        private async Task<IEnumerable<User>> GetCustomerList()
        {
            return (await _userRepository.GetAllAsync()).Where(c => c.Role == Role.Customer);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(MakeReservationViewModel makeReservation)
        {
            if (!ModelState.IsValid)
            {
                makeReservation.Cart = _cartManager.GetCartFromCookie(Request);
				return View("MakeReservationView", makeReservation);
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception();
				var customerId = int.Parse(userId);
				var reservation = new Reservation
                {
                    CustomerId = customerId,
                    PartySize = makeReservation.PartySize,
					ResDate = makeReservation.ResDate,
                    ResTime = makeReservation.ResTime,
                    Status = ReservationStatus.Confirmed,
                    Notes = makeReservation.Notes,
                };

                await _reservationRepository.InsertAsync(reservation);
            }
            catch (Exception e)
            {
                TempData["Error"] = "An error occurred while creating the reservation. Please try again later.";
				return View("MakeReservationView", makeReservation);
			}
            return RedirectToAction("Index");
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
            return View("DetailsReservationView", reservationViewModel);
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
            return View("EditReservationView", reservationViewModel);
        }

        [HttpPost]
        [Route("Edit/{ResId}")]
        public async Task<IActionResult> Edit(ReservationViewModel reservationViewModel, long ResId)
        {
            if (!ModelState.IsValid)
            {
                reservationViewModel.Customers = await GetCustomerList();
                return View("EditReservationView", reservationViewModel);
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
            return RedirectToAction("Index");
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
            return View("DeleteReservationView", reservationViewModel);
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

            return RedirectToAction("Index");
        }
    }
}
