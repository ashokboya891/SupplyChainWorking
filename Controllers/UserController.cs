using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SupplyChain.DatabaseContext;
using SupplyChain.DTOs;
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


        [HttpGet("[action]")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            if (products == null)
                return NotFound("No matching products found.");
            return Ok(products);
        }
        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderDto dto)
        {
            var parameters = new[]
            {
            new SqlParameter("@UserId", dto.UserId),
            new SqlParameter("@OrderDate", DateTime.Now),
                OrderService.GetOrderItemsTVP(dto.Items)  // Pass TVP here
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
    }
}
