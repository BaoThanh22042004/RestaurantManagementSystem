namespace WebApp.Utilities
{
	public class FileUploadManager
	{
		private readonly IWebHostEnvironment _environment;
		private readonly string _imageUploadsDir;

		public FileUploadManager(IWebHostEnvironment environment)
		{
			_environment = environment;
			_imageUploadsDir = Path.Combine(_environment.WebRootPath, "Uploads\\Images");
		}

		public async Task<string?> UploadImageAsync(IFormFile? image, string subDir, string fileName)
		{
			if (image == null || image.Length == 0)
			{
				return string.Empty;
			}

			var uploadsDir = Path.Combine(_imageUploadsDir, subDir);
			Directory.CreateDirectory(uploadsDir);

			var filePath = Path.Combine(uploadsDir, fileName);

			using var fileStream = new FileStream(filePath, FileMode.Create);
			await image.CopyToAsync(fileStream);

			return Path.Combine("/Uploads/Images", subDir, fileName);
		}

		public void DeleteImage(string subDir, string fileName)
		{
			var imagePath = Path.Combine(_imageUploadsDir, subDir, fileName);
			if (string.IsNullOrWhiteSpace(imagePath))
			{
				return;
			}

			if (File.Exists(imagePath))
			{
				File.Delete(imagePath);
			}
		}
	}
}
