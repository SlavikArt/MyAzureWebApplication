using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers
{
    public class StorageController : Controller
    {
        private readonly IAzureStorageService _storageService;
        private readonly ILogger<StorageController> _logger;

        public StorageController(
            IAzureStorageService storageService,
            ILogger<StorageController> logger)
        {
            _storageService = storageService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult UploadImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                var imageUrl = await _storageService.UploadImageAsync(file);

                await _storageService.AddMessageToQueueAsync($"Image uploaded: {file.FileName}");

                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return StatusCode(500, "Error uploading image");
            }
        }

        [HttpGet]
        public IActionResult UploadSuccess(string imageUrl)
        {
            ViewBag.ImageUrl = imageUrl;
            return View();
        }
    }
} 