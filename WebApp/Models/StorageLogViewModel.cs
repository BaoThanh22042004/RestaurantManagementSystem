using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Action = Models.Entities.Action;
namespace WebApp.Models
{
    public class StorageLogViewModel
    {
        public long? LogId { get; set; }

        public int CreatedBy { get; set; }

        [Display(Name = "Creator")]
        public string? CreatorName { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "The Change Quantity is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Change Quantity must be greater than 0.")]
        [Display(Name = "Change Quantity")]
        public decimal ChangeQuantity { get; set; }

        [Display(Name = "Remain Quantity")]
        public decimal? RemainQuantity { get; set; }

        [Required(ErrorMessage = "The Action is required.")]
        [Display(Name = "Action")]
        public Action Action { get; set; }

        [Display(Name = "Cost")]
        public decimal? Cost { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "The Item is required.")]
        [Display(Name = "Item")]
        public int ItemId { get; set; }

        // Dropdown list of items
        public IEnumerable<SelectListItem>? Items { get; set; }

        // Item name for display
        [Display(Name = "Item Name")]
        public string? ItemName { get; set; }

        // Default constructor
        public StorageLogViewModel()
        {
        }

        // Constructor that maps from the StorageLog model (entity)
        public StorageLogViewModel(StorageLog storageLog)
        {
            LogId = storageLog.LogId;
            CreatedBy = storageLog.CreatedBy;
            CreatedAt = storageLog.CreatedAt;
            ChangeQuantity = storageLog.ChangeQuantity;
            RemainQuantity = storageLog.RemainQuantity;
            Action = storageLog.Action;
            Cost = storageLog.Cost;
            Description = storageLog.Description;
            ItemId = storageLog.ItemId;
            ItemName = storageLog.StorageItem.ItemName;
            // CreatorName = storageLog.Creator.FullName;
        }
    }
}
