using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using Repositories.Interface;
using WebApp.Models;
using Action = Models.Entities.Action;

namespace WebApp.Controllers
{
    [Route("Dashboard/StorageLog")]
    [Authorize(Roles = "Manager")]
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

        private async Task<IEnumerable<SelectListItem>> GetItemistItem()
        {
            var items = await _storageRepository.GetAllAsync();
            return items.Select(item => new SelectListItem
            {
                Value = item.ItemId.ToString(),
                Text = item.ItemName
            });
        }

        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var itemViewModel = new StorageLogViewModel
            {
                Items = await GetItemistItem()
            };

            return View("CreateStorageLogView", itemViewModel);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(StorageLogViewModel storageLogViewModel)
        {
            if (!ModelState.IsValid)
            {
                storageLogViewModel.Items = await GetItemistItem();
                return View("CreateStorageLogView", storageLogViewModel);
            }

            try
            {
                var storageLog = new StorageLog
                {
                    CreatedBy = 1,
                    ItemId = storageLogViewModel.ItemId,
                    ChangeQuantity = storageLogViewModel.ChangeQuantity,
                    RemainQuantity = await UpdateRemainQuantity(storageLogViewModel),
                    Action = storageLogViewModel.Action,
                };

                await _storageLogRepository.InsertAsync(storageLog);
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "The item does not exist.";
                storageLogViewModel.Items = await GetItemistItem();
                return View("CreateStorageLogView", storageLogViewModel);
            }
            catch (InvalidDataException)
            {
                TempData["Error"] = "The remain quantity cannot be negative.";
                storageLogViewModel.Items = await GetItemistItem();
                return View("CreateStorageLogView", storageLogViewModel);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while creating storage log. Please try again later.";
                storageLogViewModel.Items = await GetItemistItem();
                return View("CreateStorageLogView", storageLogViewModel);
            }

            return RedirectToAction("Index");
        }

        [Route("Details/{id}")]
        public async Task<IActionResult> Details(long id)
        {
            var log = await _storageLogRepository.GetByIDAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            var logViewModel = new StorageLogViewModel(log);
            return View("DetailsStorageLogView", logViewModel);
        }



        [Route("Cancel/{id}")]
        public async Task<IActionResult> Cancel(long id)
        {
            var log = await _storageLogRepository.GetByIDAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            var logViewModel = new StorageLogViewModel(log);
            return View("CancelStorageLogView", logViewModel);
        }

        [HttpPost]
        [Route("Cancel/{id}")]
        public async Task<IActionResult> CancelConfirmed(long id)
        {
            try
            {
                var log = await _storageLogRepository.GetByIDAsync(id);
                if (log == null)
                {
                    return NotFound();
                }

                await UpdateRemainQuantityForCancel(log);

                log.Action = Action.Cancel;
                await _storageLogRepository.UpdateAsync(log);
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "The item does not exist.";
                return RedirectToAction("Cancel", new { id });
            }
            catch (InvalidDataException)
            {
                TempData["Error"] = "The remain quantity cannot be negative.";
                return RedirectToAction("Cancel", new { id });
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the storage log. Please try again later.";
                return RedirectToAction("Cancel", new { id });
            }

            return RedirectToAction("Index");
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

        private async Task UpdateRemainQuantityForCancel(StorageLog log)
        {
            var item = await _storageRepository.GetByIDAsync(log.ItemId);
            if (item == null)
            {
                throw new KeyNotFoundException();
            }

            decimal remainQuantity = item.Quantity;

            if (log.Action == Action.Import)
            {
                remainQuantity -= log.ChangeQuantity;
            }
            else
            {
                remainQuantity += log.ChangeQuantity;
            }

            if (remainQuantity < 0)
            {
                throw new InvalidDataException();
            }

            // Update Quantity in Storage
            item.Quantity = remainQuantity;
            await _storageRepository.UpdateAsync(item);
        }
    }
}
