using Microsoft.AspNetCore.Authorization;
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

		[Route("Create")]
		public IActionResult Create()
		{
			return View("CreateTableView");
		}

		[Route("Create")]
		[HttpPost]
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

		[Route("Details/{id}")]
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

		[Route("Edit/{id}")]
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

		[Route("Edit/{id}")]
		[HttpPost]
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

			return RedirectToAction("Index");
		}

		[Route("Delete/{id}")]
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
		[Route("Delete/{TableId}")]
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

