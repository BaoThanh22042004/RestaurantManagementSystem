using System;
using System.ComponentModel.DataAnnotations;
using Models.Entities;  // Assuming FeedBack entity and FeedBackStatus enum are part of Models.Entities namespace

namespace WebApp.Models
{
    public class FeedBackViewModel
    {
        public long? FeedbackId { get; set; }

        
        [Display(Name = "Customer ID")]
        public int? CustomerId { get; set; }


        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(255, ErrorMessage = "Email must be a maximum of 255 characters.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }


        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number must be a maximum of 15 characters.")]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [Display(Name = "Subject")]
        [StringLength(255, ErrorMessage = "Subject must be a maximum of 255 characters.")]
        public string Subject { get; set; }

        [Display(Name = "Body")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }


        [EnumDataType(typeof(FeedBackStatus), ErrorMessage = "Invalid feedback status.")]
        [Display(Name = "Status")]
        public FeedBackStatus? Status { get; set; }

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTime? CreateAt { get; set; } = DateTime.Now;

        // Default constructor
        public FeedBackViewModel()
        {
        }

        // Constructor to initialize from a FeedBack entity
        public FeedBackViewModel(FeedBack feedback)
        {
            FeedbackId = feedback.FeedbackId;
			CustomerId = feedback.CustomerId;
            Email = feedback.Email;
            Phone = feedback.Phone;
            Subject = feedback.Subject;
            Body = feedback.Body;
            Status = (FeedBackStatus)feedback.Status;
            CreateAt = feedback.CreateAt;
        }
    }
}
