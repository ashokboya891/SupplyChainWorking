using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SupplyChain.DTOs;
using SupplyChain.IServiceContracts;
using SupplyChain.Models;

namespace SupplyChain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IPhotoService _photoService;
        public ProductsController(IProductService productService,IPhotoService photoService)
        {
            _productService = productService;
            _photoService = photoService;
        }
        //[HttpPost]
        //public IActionResult AddProduct([FromBody] Product product)
        //{
        //    // Logic to add the product
        //    return Ok("Product added successfully");
        //}
        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var productDto = await _productService.GetProductByIdAsync(id);
            if (productDto == null)
                return NotFound();

            return Ok(productDto);
        }
        [HttpPost("create-with-photos")]
        public async Task<IActionResult> CreateProductWithPhotos(
            [FromForm] string productJson,
            [FromForm] List<IFormFile> photos)
        {
            // Deserialize product JSON
            var productDto = JsonConvert.DeserializeObject<ProductWithPhotosDto>(productJson);

            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                Description = productDto.Description,
                CurrentStock = productDto.CurrentStock,
                Threshold = productDto.Threshold,
                ProductTypeId = productDto.ProductTypeId,
                ProductBrandId = productDto.ProductBrandId,
                Photos = new List<ProductPhoto>()
            };

            // Save product to get ProductId
            _productService.AddProduct(product);
            //await _context.SaveChangesAsync();

            //// Upload each photo to Cloudinary and save to DB
            //foreach (var file in photos)
            //{
            //    var result = await _photoService.UploadPhotoAsync(file);
            //    var photo = new ProductPhoto
            //    {
            //        Url = result.Url,
            //        PublicId = result.PublicId,
            //        ProductId = product.ProductId,
            //        IsPrimary = false // Admin can set it later
            //    };
            //    product.Photos.Add(photo);
            //}

            //await _context.SaveChangesAsync();

            return Ok(new { message = "Product with photos added", productId = product.ProductId });
        }


        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        //{
        //    var existing = await _productService.GetProductByIdAsync(id);
        //    if (existing == null) return NotFound("Product not found");

        //    updatedProduct.ProductId = id;
        //    var result = await _productService.UpdateProductAsync(updatedProduct);
        //    if (!result) return BadRequest("Failed to update product");

        //    return Ok("Product updated successfully");
        //}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto dto)
        {
            var existing = await _productService.GetProductEntityByIdAsync(id); // ← actual EF model
            if (existing == null) return NotFound("Product not found");

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Price = dto.Price;
            existing.CurrentStock = dto.CurrentStock;
            existing.ProductTypeId = dto.ProductTypeId;
            existing.ProductBrandId = dto.ProductBrandId;

            var result = await _productService.UpdateProductAsync(existing);
            if (!result) return BadRequest("Failed to update product");

            return Ok("Product updated successfully");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound("Product not found or already deleted");

            return Ok("Product deleted successfully");
        }

        [HttpGet("product-metadata")]
        public async Task<ActionResult<ProductMetaDataDto>> GetMetaData()
        {
            return Ok(await _productService.GetProductMetaDataAsync());
        }

        [HttpPost("add-photo")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PhotoDto>> AddPhoto([FromForm] ProductPhotoUploadDto dto)
        {
            var result = await _photoService.AddProductPhotoAndSaveAsync(dto.Id, dto.File);
            if (result == null)
                return BadRequest("Failed to upload and save photo");

            return Ok(result);
        }


        //[HttpPost("add-photo/{id}")]
        //[Consumes("multipart/form-data")] // ✅ Tell Swagger this method accepts files
        //public async Task<ActionResult<PhotoDto>> AddPhoto([FromForm] ProductPhotoUploadDto dto)
        //{
        //    if (dto.File == null || dto.File.Length == 0)
        //        return BadRequest("File is required.");

        //    var existedprod = await _productService.GetProductPhotosByIdAsync(dto.Id);

        //    if (existedprod == null)
        //     return BadRequest("product no found");

        //    var result = await _photoService.AddPhotoAsync(dto.File);
        //    if (result.Error != null) return BadRequest(result.Error.Message);
        //    var photo = new ProductPhoto
        //    {
        //        Url = result.SecureUrl.AbsoluteUri,
        //        PublicId = result.PublicId
        //    };
        //    existedprod.Photos.Add(photo);

        //if (await _uow.Complete())
        //    return CreatedAtAction(nameof(GetUser),
        //    new { username = user.UserName }, _Mapper.Map<PhotoDto>(photo));
        //return BadRequest("Problem adding photo");
        //var user = await _uow.userRepository.GetUserByUserNameAsync(User.GetUsername());

        //if(user==null)
        //{
        //    return NotFound();
        //}
        //var result=await _photoService.AddPhotoAsync(file);
        //if(result.Error!=null)return BadRequest(result.Error.Message);
        //var photo=new Photo
        //{
        //    Url=result.SecureUrl.AbsoluteUri,
        //    PublicId=result.PublicId
        //};
        //if(user.Photos.Count==0)photo.IsMain=true;
        //user.Photos.Add(photo);
        //if(await _uow.Complete()) 
        //{
        //    return CreatedAtAction(nameof(GetUser),new {username=user.UserName}, _Mapper.Map<PhotoDto>(photo));
        //}
        //return BadRequest("problem adding photo");
        //}
        //[HttpPut("set-main-photo/{photoId}")]
        //public async Task<ActionResult> SetMainPhoto(int photoId)
        //{
        //    var user = await _uow.userRepository.GetUserByUserNameAsync(User.GetUsername());
        //    if (user == null) return NotFound();
        //    var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
        //    if (photo == null) NotFound();
        //    if (photo.IsMain) return BadRequest("this is already you main photo");
        //    var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        //    if (currentMain != null) currentMain.IsMain = false;
        //    photo.IsMain = true;
        //    if (await _uow.Complete()) return NoContent();
        //    return BadRequest("problem setting the main photo");
        //}

        //}
        //var user= await _uow.userRepository.GetUserByUserNameAsync(User.GetUsername());

        //var photo=user.Photos.FirstOrDefault(x=>x.Id==photoId);
        //if(photo==null)return NotFound();
        //if(photo.IsMain)return BadRequest("you can not delete your main photo");
        //if(photo.PublicId!=null)
        //{
        //    var result=await _photoService.DeletePhotoAsync(photo.PublicId)  ;
        //    if(result.Error!=null)return BadRequest(result.Error.Message);


        //}
        //user.Photos.Remove(photo);
        //if(await _uow.Complete())return Ok();
        //return BadRequest("problem deleting photo");
        //}
    }
}
