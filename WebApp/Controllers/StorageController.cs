﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Models.Entities;
using Repositories.Interface;
using WebApp.Models;
using Storage = Models.Entities.Storage;

namespace WebApp.Controllers
{
	[Route("DashBoard/Storage")]
	[Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Chef)}, {nameof(Role.Accountant)}")]
	public class StorageController : Controller
	{
		private readonly IStorageRepository _storageRepository;
		private readonly IStorageLogRepository _storageLogRepository;

		public StorageController(IStorageRepository storageRepository, IStorageLogRepository storageLogRepository)
		{
			_storageRepository = storageRepository;
			_storageLogRepository = storageLogRepository;
		}

		public async Task<IActionResult> Index()
		{
			var storages = await _storageRepository.GetAllAsync();
			var storageList = storages.Select(storage => new StorageViewModel(storage));
			return View("StorageView", storageList);
		}


		[HttpGet("Search")]
		public async Task<IActionResult> Search(string keyword)
		{
			if (keyword == null || string.IsNullOrWhiteSpace(keyword)) 
			{
				return RedirectToAction("Index", "Storage");
			}
			var storages = await _storageRepository.GetAllAsync();

			var storageList = string.IsNullOrWhiteSpace(keyword)
				? storages.Select(storage => new StorageViewModel(storage)).ToList()
				: storages
					.Where(s => s.ItemName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
					.Select(storage => new StorageViewModel(storage))
					.ToList();

			return View("StorageView", storageList);
		}

		[HttpGet("Create")]
		public IActionResult Create()
		{
			return PartialView("_CreateStorageModal");
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create(StorageViewModel storageViewModel)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_CreateStorageModal", storageViewModel);
			}

			try
			{
				var storage = new Storage
				{
					ItemName = storageViewModel.ItemName,
					Unit = storageViewModel.Unit,
					Quantity = 0
				};

				await _storageRepository.InsertAsync(storage);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while creating the storage item. Please try again.";
				return PartialView("_CreateStorageModal", storageViewModel);
			}

			return Json(new { success = true });
		}

		[Route("Details/{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var storage = await _storageRepository.GetByIDAsync(id);
			if (storage == null)
			{
				return NotFound();
			}

			var logOfItem = await _storageLogRepository.GetAllByItemIdAsync(id);

			var detailedStorageViewModel = new StorageWithLogViewModel(storage);
			detailedStorageViewModel.Logs = logOfItem.Select(log => new StorageLogViewModel(log));
			return View("DetailsStorageView", detailedStorageViewModel);
		}

		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var storage = await _storageRepository.GetByIDAsync(id);
			if (storage == null)
			{
				return NotFound();
			}

			var storageViewModel = new StorageViewModel(storage);
			return PartialView("_EditStorageModal", storageViewModel);
		}

		[HttpPost]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(StorageViewModel storageViewModel)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_EditStorageModal", storageViewModel);
			}

			try
			{
				var id = storageViewModel.ItemId ?? throw new KeyNotFoundException();
				var storageEntity = await _storageRepository.GetByIDAsync(id);
				if (storageEntity == null)
				{
					throw new KeyNotFoundException();
				}

				storageEntity.ItemName = storageViewModel.ItemName;
				storageEntity.Unit = storageViewModel.Unit;
				storageEntity.Quantity = storageViewModel.Quantity;

				await _storageRepository.UpdateAsync(storageEntity);
			}
			catch (KeyNotFoundException)
			{
				return NotFound();
			}
			catch (Exception e)
			{
				TempData["Error"] = "An error occurred while updating the storage item. Please try again later.";
				return PartialView("_EditStorageModal", storageViewModel);
			}

			return Json(new { success = true });
		}

		[Route("Delete/{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var storage = await _storageRepository.GetByIDAsync(id);
			if (storage == null)
			{
				return NotFound();
			}

			var storageViewModel = new StorageViewModel(storage);
			return PartialView("_DeleteStorageModal", storageViewModel);
		}

		[HttpPost]
		[Route("Delete/{ItemId}")]
		public async Task<IActionResult> DeleteConfirmed(int ItemId)
		{
			try
			{
				await _storageRepository.DeleteAsync(ItemId);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while deleting the storage item. Please try again later.";
				return RedirectToAction("Delete", new { id = ItemId });
			}

			return Json(new { success = true });
		}
	}
}
