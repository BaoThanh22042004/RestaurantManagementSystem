using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Repositories;
using Repositories.Interface;
using WebApp.Models;

namespace WebApp.Controllers
{
	[Route("Dashboard")]
	[Authorize(Roles = "Manager, Waitstaff")]
	public class TableController : Controller
	{
		private readonly ITableRepository _tableRepository;

		public TableController(ITableRepository tableRepository)
		{
			_tableRepository = tableRepository;
		}

		// GET: Dashboard/Table
		[Route("Table")]
		public async Task<IActionResult> Index()
		{
			var tables = await _tableRepository.GetAllAsync();
			var tableList = tables.Select(table => new TableViewModel(table));
			return View("TableView", tableList);
		}

		// GET: Dashboard/Table/Create
		[Route("Table/Create")]
		public IActionResult Create()
		{
			return View("CreateTableView");
		}

		// POST: Dashboard/Table/Create
		[Route("Table/Create")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(TableViewModel tableViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View("CreateTableView", tableViewModel);
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
				return View("CreateTableView", tableViewModel);
			}

			return RedirectToAction("Index");
		}

		// GET: Dashboard/Table/Details/{id}
		[Route("Table/Details/{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var table = await _tableRepository.GetByIDAsync(id);
			if (table == null)
			{
				return NotFound();
			}

			var tableViewModel = new TableViewModel(table);
			return View("DetailsTableView", tableViewModel);
		}

		// GET: Dashboard/Table/Edit/{id}
		[Route("Table/Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var table = await _tableRepository.GetByIDAsync(id);
			if (table == null)
			{
				return NotFound();
			}

			var tableViewModel = new TableViewModel(table);
			return View("EditTableView", tableViewModel);
		}

		// POST: Dashboard/Table/Edit/{id}
		[Route("Table/Edit/{TableId}")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(TableViewModel tableViewModel, int TableId)
		{
			if (!ModelState.IsValid)
			{
				return View("EditTableView", tableViewModel);
			}

			try
			{
				var tableEntity = await _tableRepository.GetByIDAsync(TableId);
				if (tableEntity == null)
				{
					return NotFound();
				}

				tableEntity.TableName = tableViewModel.TableName;
				tableEntity.Capacity = tableViewModel.Capacity;
				tableEntity.Status = tableViewModel.Status;
				tableEntity.ResTime = tableViewModel.ResTime;
				tableEntity.Notes = tableViewModel.Notes;

				await _tableRepository.UpdateAsync(tableEntity);
			}
			catch (Exception)
			{
				TempData["Error"] = "An error occurred while updating the table. Please try again later.";
				return View("EditTableView", tableViewModel);
			}

			return RedirectToAction("Index");
		}


        [Route("Table/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var table = await _tableRepository.GetByIDAsync(id);
            if (table == null)
            {
                return NotFound();
            }

			var tableViewModel = new TableViewModel(table);
            return View("DeleteTableView", tableViewModel);
        }

        [HttpPost]
        [Route("Table/Delete/{TableId}")]
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

            return RedirectToAction("Index");
        }
    }
}

