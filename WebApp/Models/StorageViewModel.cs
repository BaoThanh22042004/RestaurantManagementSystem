﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;

namespace WebApp.Models
{
    public class StorageViewModel
    {
        [Display(Name = "Item ID")]
        public int? ItemId { get; set; }

        [Required(ErrorMessage = "Item name is required.")]
        [StringLength(255, ErrorMessage = "Item name must not exceed 255 characters.")]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        [StringLength(100, ErrorMessage = "Unit must not exceed 100 characters.")]
        [Display(Name = "Unit")]
        public string Unit { get; set; }

        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

        public StorageViewModel() { }

		public StorageViewModel(Storage storage)
        {
            ItemId = storage.ItemId;
            ItemName = storage.ItemName;
            Unit = storage.Unit;
            Quantity = storage.Quantity;
        }
    }
}
