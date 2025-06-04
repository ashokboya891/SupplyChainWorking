using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupplyChain.IServiceContracts;

namespace SupplyChain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IProductService _productService;
        public UploadController(IProductService productService)
        {
            _productService = productService;
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
        //[HttpPost]
        //[Route("UploadExcelForm")]

        //public async Task<IActionResult> UploadExcelForm(IFormFile excelFile)
        //{
        //    if (excelFile == null || excelFile.Length == 0)
        //    {
        //        return BadRequest("File Corrupted");
        //    }
        //    if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        //    {
        //        return BadRequest("unsupported  file xlsx is expected\"");
        //    }
        //    int count = await countryService.UploadCountriesFromExcelFile(excelFile);
        //    ViewBag.Message = $"{count} number of countries uploaded";
        //    return View();
        //}
    }
}
