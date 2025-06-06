using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupplyChain.DatabaseContext;
using SupplyChain.IServiceContracts;
using SupplyChain.Models;
using System.Security.Claims;

namespace SupplyChain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;
        public UploadController(IProductService productService,ApplicationDbContext context)
        {
            _productService = productService;
            this._context = context;
        }
        [HttpPost("upload-excel")]
        public async Task<IActionResult> UploadExcelFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            try
            {
                int productsInserted = await _productService.UploadOrdersFromExcelFile(file);
                return Ok(new { Message = $"{productsInserted} products inserted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("save-excel")]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", userId);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Optional: Save file info in DB
             await _context.UploadedFiles.AddAsync(new UploadedFile { UserId = userId, FileName = uniqueFileName });
             await _context.SaveChangesAsync();

            return Ok(new { message = "File uploaded successfully", filePath });
        }

        [Authorize]
        [HttpGet("my-files")]
        public IActionResult GetMyFiles()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", userId);

            if (!Directory.Exists(userFolder))
                return Ok(new List<string>());

            var files = Directory.GetFiles(userFolder)
                                 .Select(f => Path.GetFileName(f))
                                 .ToList();

            return Ok(files);
        }

        [Authorize]
        [HttpGet("download-file/{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", userId, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; // MIME for .xlsx
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, contentType, fileName);
        }



    }
}
