using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using WebApp.Models;

namespace WebApp.Controllers
{
	[Route("DashBoard/Storage")]
	[Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Chef)}, {nameof(Role.Accountant)}")]
	public class StorageController : Controller
	{
		private readonly IStorageRepository _storageRepository;

		public StorageController(IStorageRepository storageRepository)
		{
			_storageRepository = storageRepository;
		}

		public async Task<IActionResult> Index()
		{
			var storages = await _storageRepository.GetAllAsync();
			var storageList = storages.Select(storage => new StorageViewModel(storage));
			return View("StorageView", storageList);
		}

		[Route("Create")]
		public IActionResult Create()
		{
			return View("CreateStorageView");
		}

		[Route("Create")]
		[HttpPost]
		public async Task<IActionResult> Create(StorageViewModel storageViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View("CreateStorageView", storageViewModel);
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
				return View("CreateStorageView", storageViewModel);
			}

			return RedirectToAction("Index");
		}

		[Route("Details/{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var storage = await _storageRepository.GetByIDAsync(id);
			if (storage == null)
			{
				return NotFound();
			}
			var storageViewModel = new StorageViewModel(storage);
			return View("DetailsStorageView", storageViewModel);
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
			return View("EditStorageView", storageViewModel);
		}

		[HttpPost]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(StorageViewModel storageViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View("EditStorageView", storageViewModel);
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
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while updating the storage item. Please try again later.";
				return View("EditStorageView", storageViewModel);
			}

			return RedirectToAction("Index");
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
			return View("DeleteStorageView", storageViewModel);
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

			return RedirectToAction("Index");
		}
	}
}
