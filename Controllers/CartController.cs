using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupplyChain.DTOs;
using SupplyChain.IServiceContracts;
using SupplyChain.Models;

namespace SupplyChain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] CartItemRequest request)
        {
            await _cartService.AddOrUpdateItemAsync(request.UserId, new CartItem
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                ProductName = request.ProductName,
                UnitPrice = request.UnitPrice
            });

            return Ok("Added to cart");
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(Guid userId)
        {
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> ClearCart(Guid userId)
        {
            await _cartService.ClearCartAsync(userId);
            return Ok("Cart cleared");
        }


        [HttpPost("{userId}/{itemId}")]
        public async Task<IActionResult> RemoveItem(Guid userId,int itemId)
        {
            await _cartService.RemoveItemFromCart(userId, itemId);
            return Ok("item removed ");
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateQuantity(Guid userId, int itemId, [FromBody]int quantity)
        {
            await _cartService.UpdateQuantity(userId, itemId,quantity);
            return Ok("cart updated");
        }
    }
}
