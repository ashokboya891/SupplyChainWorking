using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using SupplyChain.DatabaseContext;
using SupplyChain.DTOs;
using SupplyChain.Helpers;
using SupplyChain.Models;
using SupplyChain.Services;
using System.Security.Claims;

namespace SupplyChain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            this._context = context;
        }


        //[HttpGet("[action]")]
        //public async Task<IActionResult> GetProducts([FromQuery] int? typeId, [FromQuery] int? brandId)
        //{
        //    var products = await _context.Products.ToListAsync();
        //    if (products == null)
        //        return NotFound("No matching products found.");
        //    return Ok(products);
        //}

        [HttpGet("[action]")]
        [Cached(600)]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryParams queryParams)
        {
            var query = _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.ProductBrand).
                Include(p=>p.Photos)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                var searchLower = queryParams.Search.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    p.Description.ToLower().Contains(searchLower));
            }

            // Filters
            if (queryParams.TypeId.HasValue)
                query = query.Where(p => p.ProductTypeId == queryParams.TypeId);

            if (queryParams.BrandId.HasValue)
                query = query.Where(p => p.ProductBrandId == queryParams.BrandId);

            // Sorting
            query = queryParams.Sort switch
            {
                "priceAsc" => query.OrderBy(p => p.Price),
                "priceDesc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Name)
            };

            // Pagination
            var totalItems = await query.CountAsync();

            var products = await query
                .Skip((queryParams.PageIndex - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            // 🔁 Manual Mapping to DTO
            //var data = products.Select(p => new ProductToReturnDto
            //{
            //    Id = p.ProductId,
            //    Name = p.Name,
            //    Description = p.Description,
            //    Price = p.Price,
            //    ProductBrand = p.ProductBrand?.Name,
            //    ProductType = p.ProductType?.Name,
            //    CurrentStock = p.CurrentStock.ToString(), // Assuming CurrentStock is a property of Product
            //    PrimaryImageUrl = p.Photos.FirstOrDefault(photo => photo.IsPrimary).Url,
            //    AllImages = p.Photos.Select(photo => photo.Url).ToList()
            //}).ToList();
            var data = products.Select(p => new ProductToReturnDto
            {
                Id = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ProductBrand = p.ProductBrand?.Name,
                ProductType = p.ProductType?.Name,
                CurrentStock = p.CurrentStock.ToString(),

                PrimaryImageUrl = p.Photos.FirstOrDefault(photo => photo.IsPrimary)?.Url,
                AllImages = p.Photos.Select(photo => photo.Url).ToList()
            }).ToList();


            var result = new
            {
                pageIndex = queryParams.PageIndex,
                pageSize = queryParams.PageSize,
                totalCount = totalItems,
                data
            };

            return Ok(result);
        }


        //[HttpGet("[action]")]
        //[Cached(600)] // ⏱ Redis cache for 10 minutes
        //public async Task<IActionResult> GetProducts([FromQuery] ProductQueryParams queryParams)
        //{
        //    var query = _context.Products
        //        .Include(p => p.ProductType)
        //        .Include(p => p.ProductBrand)
        //        .AsQueryable();

        //    // 🔍 Search
        //    if (!string.IsNullOrEmpty(queryParams.Search))
        //    {
        //        var searchLower = queryParams.Search.ToLower();
        //        query = query.Where(p => p.Name.ToLower().Contains(searchLower) || p.Description.ToLower().Contains(searchLower));
        //    }

        //    // 📂 Filter by Type or Brand
        //    if (queryParams.TypeId.HasValue)
        //        query = query.Where(p => p.ProductTypeId == queryParams.TypeId);

        //    if (queryParams.BrandId.HasValue)
        //        query = query.Where(p => p.ProductBrandId == queryParams.BrandId);

        //    // 🔁 Sorting
        //    query = queryParams.Sort switch
        //    {
        //        "priceAsc" => query.OrderBy(p => p.Price),
        //        "priceDesc" => query.OrderByDescending(p => p.Price),
        //        _ => query.OrderBy(p => p.Name)
        //    };

        //    // 📄 Pagination
        //    var totalItems = await query.CountAsync();
        //    var products = await query
        //        .Skip((queryParams.PageIndex - 1) * queryParams.PageSize)
        //        .Take(queryParams.PageSize)
        //        .ToListAsync();

        //    // 📦 Return paged response
        //    var result = new
        //    {
        //        pageIndex = queryParams.PageIndex,
        //        pageSize = queryParams.PageSize,
        //        totalCount = totalItems,
        //        data = products
        //    };

        //    return Ok(result);
        //}

        //[Cached(600)]
        //[HttpGet]
        //public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductsSpecParams productsSpecParams)  //instead of writing like this (string sort,int? brandId,int? typeId ) we minimised out code by adding new class core.specifications.PorductsParams
        //{
        //    var spec = new ProductsWithTypesAndBrandsSpecification(productsSpecParams);
        //    var countSpec = new ProductWithFiltersForCountSpecification(productsSpecParams);
        //    var totalItems = await _productRepo.CountAsync(countSpec);
        //    var products = await _productRepo.ListAsync(spec);
        //    var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);
        //    return Ok(new Pagination<ProductToReturnDto>(productsSpecParams.PageIndex, productsSpecParams.PageSize, totalItems, data));
        //}

        //[HttpGet("[action]")]
        //public async Task<ActionResult<List<ProductDto>>> GetProducts([FromQuery] int? typeId, [FromQuery] int? brandId)
        //{
        //    var query = _context.Products
        //        .Include(p => p.ProductType)
        //        .Include(p => p.ProductBrand)
        //        .AsQueryable();

        //    if (typeId.HasValue)
        //        query = query.Where(p => p.ProductTypeId == typeId.Value);

        //    if (brandId.HasValue)
        //        query = query.Where(p => p.ProductBrandId == brandId.Value);

        //    var products = await query.ToListAsync();

        //    return Ok(products);
        //}

        //[HttpGet("types")]
        //public async Task<ActionResult<List<ProductType>>> GetTypes(int typeId)
        //{
        //    return await _context.ProductTypes.Include(p => p.Products).Where(o => o.Id == typeId).ToListAsync();
        //}

        //[HttpGet("brands")]
        //public async Task<ActionResult<List<ProductBrand>>> GetBrands(int brandId)
        //{
        //    return await _context.ProductBrands.Include(p=>p.Products).Where(o=>o.Id == brandId).ToListAsync();
        //}


        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderDto dto)
        {
            var parameters = new[]
            {
            new SqlParameter("@UserId", dto.UserId),
            new SqlParameter("@OrderDate", DateTime.Now),
                ProductService.GetOrderItemsTVP(dto.Items)  // Pass TVP here
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC PlaceOrder @UserId, @OrderDate, @OrderItems", parameters);

            return Ok("Order placed successfully.");
        }


        [HttpGet("get-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");
            //        var ordersWithProducts = _context.Orders
            //.Where(o => o.UserId.ToString() == userId) // filter by user if needed
            //.Include(o => o.OrderItems)
            //    .ThenInclude(oi => oi.Product)
            //.ToList();
            var orders = _context.Orders
                .Where(o => o.UserId.ToString() == userId) // filter by specific user
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    RazorpayOrderId = o.RazorpayOrderId,
                    RazorpayPaymentId = o.RazorpayPaymentId,
                    PaidAt = o.PaidAt,
                    PaymentStatus = o.PaymentStatus,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList()
                })
                .ToList();

            return Ok(orders);
        }

        [Cached(600)]
        [HttpGet("brands")]
        // 
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _context.ProductBrands.ToListAsync());

        }


        [Cached(600)]
        [HttpGet("types")]
        // 
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetTypesBrands()
        {
            return Ok(await _context.ProductTypes.ToListAsync());

        }
    }
}
