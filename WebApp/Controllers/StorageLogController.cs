using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using System.Security.Claims;
using WebApp.Models;
using Action = Models.Entities.Action;

namespace WebApp.Controllers
{
	[Route("Dashboard/StorageLog")]
	[Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Chef)}, {nameof(Role.Accountant)}")]
	public class StorageLogController : Controller
	{
		private readonly IStorageLogRepository _storageLogRepository;
		private readonly IStorageRepository _storageRepository;

		public StorageLogController(IStorageLogRepository storageLogRepository, IStorageRepository storageRepository)
		{
			_storageLogRepository = storageLogRepository;
			_storageRepository = storageRepository;
		}

		public async Task<IActionResult> Index()
		{
			var logs = await _storageLogRepository.GetAllAsync();
			var logList = logs.Select(log => new StorageLogViewModel(log));
			return View("StorageLogView", logList);
		}


		[HttpGet("Import/{id}")]
		public IActionResult Import(int id)
		{
			var storageLogViewModel = new StorageLogViewModel
			{
				ItemId = id,
				Action = Action.Import,
			};
			return PartialView("_ImportItemModal", storageLogViewModel);
		}

		[HttpPost("Import/{id}")]
		public async Task<IActionResult> Import(StorageLogViewModel storageLogViewModel)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_ImportItemModal", storageLogViewModel);
			}

			try
			{
				var storageLog = new StorageLog
				{
					CreatedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
					CreatedAt = DateTime.Now,
					ItemId = storageLogViewModel.ItemId,
					ChangeQuantity = storageLogViewModel.ChangeQuantity,
					RemainQuantity = await UpdateRemainQuantity(storageLogViewModel),
					Action = storageLogViewModel.Action,
					Cost = storageLogViewModel.Cost,
					Description = storageLogViewModel.Description,
				};

				await _storageLogRepository.InsertAsync(storageLog);
			}
			catch (KeyNotFoundException)
			{
				TempData["Error"] = "The item does not exist.";
				return PartialView("_ImportItemModal", storageLogViewModel);
			}
			catch (InvalidDataException)
			{
				TempData["Error"] = "The remain quantity cannot be negative.";
				return PartialView("_ImportItemModal", storageLogViewModel);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while importing storage log. Please try again later.";
				return PartialView("_ImportItemModal", storageLogViewModel);
			}

			return Json(new { success = true });
		}

		[HttpGet("Export/{id}")]
		public IActionResult Export(int id)
		{
			var storageLogViewModel = new StorageLogViewModel
			{
				ItemId = id,
				Action = Action.Export,
			};
			return PartialView("_ExportItemModal", storageLogViewModel);
		}

		[HttpPost("Export/{id}")]
		public async Task<IActionResult> Export(StorageLogViewModel storageLogViewModel)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_ExportItemModal", storageLogViewModel);
			}

			try
			{
				var storageLog = new StorageLog
				{
					CreatedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
					CreatedAt = DateTime.Now,
					ItemId = storageLogViewModel.ItemId,
					ChangeQuantity = storageLogViewModel.ChangeQuantity,
					RemainQuantity = await UpdateRemainQuantity(storageLogViewModel),
					Action = storageLogViewModel.Action,
					Description = storageLogViewModel.Description,
				};

				await _storageLogRepository.InsertAsync(storageLog);
			}
			catch (KeyNotFoundException)
			{
				TempData["Error"] = "The item does not exist.";
				return PartialView("_ExportItemModal", storageLogViewModel);
			}
			catch (InvalidDataException)
			{
				TempData["Error"] = "The remain quantity cannot be negative.";
				return PartialView("_ExportItemModal", storageLogViewModel);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while importing storage log. Please try again later.";
				return PartialView("_ExportItemModal", storageLogViewModel);
			}

			return Json(new { success = true });
		}

		[HttpGet("Details/{id}")]
		public async Task<IActionResult> Details(long id)
		{
			var log = await _storageLogRepository.GetByIDAsync(id);
			if (log == null)
			{
				return NotFound();
			}

			var logViewModel = new StorageLogViewModel(log);
			return PartialView("_DetailsItemLogModal", logViewModel);
		}

		private async Task<decimal> UpdateRemainQuantity(StorageLogViewModel storageLogViewModel)
		{
			var item = await _storageRepository.GetByIDAsync(storageLogViewModel.ItemId);
			if (item == null)
			{
				throw new KeyNotFoundException();
			}

			decimal remainQuantity = item.Quantity;

			if (storageLogViewModel.Action == Action.Import)
			{
				remainQuantity += storageLogViewModel.ChangeQuantity;
			}
			else
			{
				remainQuantity -= storageLogViewModel.ChangeQuantity;
			}

			if (remainQuantity < 0)
			{
				throw new InvalidDataException();
			}

			// Update Quantity in Storage
			item.Quantity = remainQuantity;
			await _storageRepository.UpdateAsync(item);

			return remainQuantity;
		}
	}
}
