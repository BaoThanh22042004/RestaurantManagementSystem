using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class ReservationViewModel
    {
        [Display(Name = "Reservation ID")]
        public long? ResId { get; set; }

        [Display(Name = "Customer ID")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Party size is required.")]
        [Range(1, short.MaxValue, ErrorMessage = "Party size must be at least 1.")]
        [Display(Name = "Party Size")]
        public short PartySize { get; set; }

        [Required(ErrorMessage = "Reservation date is required.")]
        [Display(Name = "Reservation Date")]
        public DateOnly ResDate { get; set; }

        [Required(ErrorMessage = "Reservation time is required.")]
        [Display(Name = "Reservation Time")]
        public TimeOnly ResTime { get; set; }

        [Required(ErrorMessage = "Reservation status is required.")]
        [Display(Name = "Reservation Status")]
        public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;

        [Display(Name = "Deposit Amount")]
        public decimal? DepositAmount { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot be longer than 500 characters.")]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public IEnumerable<User>? Customers { get; set; }

        public User? Customer { get; set; }

        // Constructors
        public ReservationViewModel()
        {
        }

        public ReservationViewModel(Reservation reservation)
        {
            ResId = reservation.ResId;
            CustomerId = reservation.CustomerId;//
            PartySize = reservation.PartySize;//
            ResDate = reservation.ResDate;//
            ResTime = reservation.ResTime;//
            Status = reservation.Status;
            DepositAmount = reservation.DepositAmount;//
            Notes = reservation.Notes;//
            Customer = reservation.Customer;//
        }
    }
}
