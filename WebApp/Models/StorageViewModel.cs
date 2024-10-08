//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using Models.Entities;
//using Microsoft.EntityFrameworkCore;

//namespace WebApp.Models
//{
//	public class StorageViewModel
//	{
//		[Display(Name = "Item ID")]
//		public int? ItemId { get; set; }

//		[Required(ErrorMessage = "Item name is required.")]
//		[StringLength(255, ErrorMessage = "Item name must be between 1 and 255 characters.")]
//		[Display(Name = "Item Name")]
//		public string ItemName { get; set; }

//		[Required(ErrorMessage = "Unit is required.")]
//		[StringLength(100, ErrorMessage = "Unit must be between 1 and 100 characters.")]
//		[Display(Name = "Unit")]
//		public string Unit { get; set; }

//		[Required(ErrorMessage = "Quantity is required.")]
//		[Range(0, 1000000, ErrorMessage = "Quantity must be a positive value and not exceed 1,000,000.")]
//		[Precision(10, 2)]
//		[Display(Name = "Quantity")]
//		public decimal Quantity { get; set; }


//		// Default constructor
//		public StorageViewModel()
//		{
//		}

//		// Constructor that maps from the Storage model (entity)
//		public StorageViewModel(Storage storage)
//		{
//			ItemId = storage.ItemId;
//			ItemName = storage.ItemName;
//			Unit = storage.Unit;
//			Quantity = storage.Quantity;

//			// Map StorageLog entities to StorageLogViewModel collection
//			foreach (var log in storage.StorageLogs)
//			{
//				StorageLogs.Add(new StorageLogViewModel(log));
//			}
//		}

//		// Method to convert back to Storage entity (for database operations)
//		public Storage ToEntity()
//		{
//			var storageEntity = new Storage
//			{
//				ItemId = this.ItemId ?? 0, // Set to 0 for new entries
//				ItemName = this.ItemName,
//				Unit = this.Unit,
//				Quantity = this.Quantity,
//			};

//			return storageEntity;
//		}
//	}
//}
