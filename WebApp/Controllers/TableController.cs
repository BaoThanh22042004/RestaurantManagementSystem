﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories.Interface;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("Dashboard/Table")]
    [Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Waitstaff)}")]
    public class TableController : Controller
    {
        private readonly ITableRepository _tableRepository;

        public TableController(ITableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }

        public async Task<IActionResult> Index()
        {
            var tables = await _tableRepository.GetAllAsync();
            var tableList = tables.Select(table => new TableViewModel(table));
            return View("TableView", tableList);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
			return PartialView("_CreateTableModal");
		}

      
        [HttpPost("Create")]
        public async Task<IActionResult> Create(TableViewModel tableViewModel)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_CreateTableModal", tableViewModel);
			}

            try
            {
                var table = new Table
                {
                    TableName = tableViewModel.TableName,
                    Capacity = tableViewModel.Capacity,
                    Status = tableViewModel.Status,
                    ResTime = tableViewModel.ResTime,
                    Notes = tableViewModel.Notes
                };

                await _tableRepository.InsertAsync(table);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while creating the table. Please try again later.";
				return PartialView("_CreateTableModal", tableViewModel);
			}
			return Json(new { success = true });
		}

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var table = await _tableRepository.GetByIDAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            var tableViewModel = new TableViewModel(table);
            return PartialView("_DetailsTableModal", tableViewModel);
		}

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var table = await _tableRepository.GetByIDAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            var tableViewModel = new TableViewModel(table);
           return PartialView("_EditTableModal", tableViewModel);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(TableViewModel tableViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("EditTableView", tableViewModel);
            }

            try
            {
                var id = tableViewModel.TableId ?? throw new KeyNotFoundException();
                var tableEntity = await _tableRepository.GetByIDAsync(id);
                if (tableEntity == null)
                {
                    throw new KeyNotFoundException();
                }

                tableEntity.TableName = tableViewModel.TableName;
                tableEntity.Capacity = tableViewModel.Capacity;
                tableEntity.Status = tableViewModel.Status;
                tableEntity.ResTime = tableViewModel.ResTime;
                tableEntity.Notes = tableViewModel.Notes;

                await _tableRepository.UpdateAsync(tableEntity);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while updating the table. Please try again later.";
                return View("EditTableView", tableViewModel);
            }
            return Json(new { success = true });

        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var table = await _tableRepository.GetByIDAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            var tableViewModel = new TableViewModel(table);
            return PartialView("_DeleteTableModal", tableViewModel);
        }

        [HttpPost("Delete/{TableId}")]
        public async Task<IActionResult> DeleteConfirmed(int TableId)
        {
            try
            {
                await _tableRepository.DeleteAsync(TableId);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting user. Please try again later.";
                return RedirectToAction("Delete", new { TableId });
            }

            return Json(new { success = true });
        }
    }
}

