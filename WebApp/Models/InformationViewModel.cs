using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
	public class ContentItem()
	{
		public string ImagePath { get; set; } = string.Empty;
		[Required(ErrorMessage = "Heading is required.")]
		[Display(Name = "Heading")]
		public string Heading { get; set; } = string.Empty;
		[Required(ErrorMessage = "Secondary Heading is required.")]
		[Display(Name = "Secondary Heading")]
		public string SecondaryHeading { get; set; } = string.Empty;
		[Required(ErrorMessage = "Description is required.")]
		public string Description { get; set; } = string.Empty;
	}

	public class Contact()
	{
		[Required(ErrorMessage = "Call to action is required.")]
		[Display(Name = "Call to Action")]
		public string CTA { get; set; } = string.Empty;
		[Required(ErrorMessage = "Address is required.")]
		public string Address { get; set; } = string.Empty;
		[Required(ErrorMessage = "Email is required.")]
		public string Email { get; set; } = string.Empty;
		[Required(ErrorMessage = "Phone is required.")]
		public string Phone { get; set; } = string.Empty;
	}

	public class InformationViewModel
	{
		public List<ContentItem> Carousels { get; set; } = [];
		public List<ContentItem> Highlights { get; set; } = [];
		public List<ContentItem> Features { get; set; } = [];
		public Contact Contact { get; set; } = new Contact();

		[Required(ErrorMessage = "Restaurant name is required.")]
		public string RestaurantName { get; set; } = string.Empty;
	}
}
