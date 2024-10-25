using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualBasic;
using Models.Entities;
using WebApp.Models;
using WebApp.Utilities;

namespace WebApp.Controllers
{
	[Route("Dashboard/Information")]
	[Authorize(Roles = $"{nameof(Role.Manager)}")]
	public class InformationController : Controller
	{
		private readonly InformationManager _informationManager;
		private readonly FileUploadManager _fileUploadManager;

		public InformationController(InformationManager informationManager, FileUploadManager fileUploadManager)
		{
			_informationManager = informationManager;
			_fileUploadManager = fileUploadManager;
		}

		public IActionResult Index()
		{
			var information = _informationManager.Information;

			return View("InformationView", information);
		}

		[HttpGet("RestaurantName")]
		public IActionResult RestaurantName()
		{
			return PartialView("_RestaurantNameModal", _informationManager.Information);
		}

		[HttpPost("RestaurantName")]
		public async Task<IActionResult> RestaurantName(InformationViewModel informationViewModel)
		{
			if (ModelState.GetValidationState(nameof(informationViewModel.RestaurantName)) != ModelValidationState.Valid)
			{
				return PartialView("_RestaurantNameModal", informationViewModel);
			}

			_informationManager.Information.RestaurantName = informationViewModel.RestaurantName;
			await _informationManager.SaveInformation();
			return Json(new { success = true });
		}

		[HttpGet("Carousel/{index}")]
		public IActionResult Carousel(int index)
		{
			var carousels = _informationManager.Information.Carousels;
			if (index < 0 || index >= carousels.Count)
			{
				return NotFound();
			}
			var carousel = _informationManager.Information.Carousels[index];

			return PartialView("_CarouselModal", carousel);
		}

		[HttpPost("Carousel/{index}")]
		public async Task<IActionResult> Carousel(int index, ContentItem carouselItem, IFormFile? file)
		{
			ModelState.MarkFieldSkipped(nameof(carouselItem.SecondaryHeading));
			if (!ModelState.IsValid)
			{
				return PartialView("_CarouselModal", carouselItem);
			}

			var carousels = _informationManager.Information.Carousels;
			if (index < 0 || index >= carousels.Count)
			{
				return NotFound();
			}

			if (file != null && file.Length > 0)
			{
				// Check if file is not image
				if (!file.ContentType.Contains("image"))
				{
					ModelState.AddModelError(nameof(file), "File must be an image.");
					return PartialView("_CarouselModal", carouselItem);
				}

				var imagePath = await _fileUploadManager.UploadImageAsync(file, "Home", $"Carousel-{index + 1}.jpg");
				_informationManager.Information.Carousels[index].ImagePath = imagePath + "?" + DateTime.Now.Ticks;
			}

			carousels[index].Heading = carouselItem.Heading;
			carousels[index].Description = carouselItem.Description;
			await _informationManager.SaveInformation();

			return Json(new { success = true });
		}

		[HttpGet("Highlight/{index}")]
		public IActionResult Highlight(int index)
		{
			var highlights = _informationManager.Information.Highlights;
			if (index < 0 || index >= highlights.Count)
			{
				return NotFound();
			}
			var highlight = _informationManager.Information.Highlights[index];

			return PartialView("_HighlightModal", highlight);
		}

		[HttpPost("Highlight/{index}")]
		public async Task<IActionResult> Highlight(int index, ContentItem highlightItem, IFormFile? file)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_HighlightModal", highlightItem);
			}

			var highlights = _informationManager.Information.Highlights;
			if (index < 0 || index >= highlights.Count)
			{
				return NotFound();
			}

			if (file != null && file.Length > 0)
			{
				// Check if file is not image
				if (!file.ContentType.Contains("image"))
				{
					ModelState.AddModelError(nameof(file), "File must be an image.");
					return PartialView("_HighlightModal", highlightItem);
				}

				var imagePath = await _fileUploadManager.UploadImageAsync(file, "Home", $"Highlight-{index + 1}.jpg");
				_informationManager.Information.Highlights[index].ImagePath = imagePath + "?" + DateTime.Now.Ticks;
			}

			highlights[index].Heading = highlightItem.Heading;
			highlights[index].Description = highlightItem.Description;
			await _informationManager.SaveInformation();

			return Json(new { success = true });
		}

		[HttpGet("Feature/{index}")]
		public IActionResult Feature(int index)
		{
			var features = _informationManager.Information.Features;
			if (index < 0 || index >= features.Count)
			{
				return NotFound();
			}
			var feature = _informationManager.Information.Features[index];
			return PartialView("_FeatureModal", feature);
		}

		[HttpPost("Feature/{index}")]
		public async Task<IActionResult> Feature(int index, ContentItem featureItem, IFormFile? file)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_FeatureModal", featureItem);
			}

			var features = _informationManager.Information.Features;
			if (index < 0 || index >= features.Count)
			{
				return NotFound();
			}

			if (file != null && file.Length > 0)
			{
				// Check if file is not image
				if (!file.ContentType.Contains("image"))
				{
					ModelState.AddModelError(nameof(file), "File must be an image.");
					return PartialView("_FeatureModal", featureItem);
				}

				var imagePath = await _fileUploadManager.UploadImageAsync(file, "Home", $"Feature-{index + 1}.jpg");
				_informationManager.Information.Features[index].ImagePath = imagePath + "?" + DateTime.Now.Ticks;
			}

			features[index].Heading = featureItem.Heading;
			features[index].SecondaryHeading = featureItem.SecondaryHeading;
			features[index].Description = featureItem.Description;
			await _informationManager.SaveInformation();

			return Json(new { success = true });
		}

		[HttpGet("Contact")]
		public IActionResult Contact()
		{
			var contact = _informationManager.Information.Contact;
			return PartialView("_ContactModal", contact);
		}

		[HttpPost("Contact")]
		public async Task<IActionResult> Contact(Contact contact)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("_ContactModal", contact);
			}

			_informationManager.Information.Contact = contact;
			await _informationManager.SaveInformation();

			return Json(new { success = true });
		}
	}
}
