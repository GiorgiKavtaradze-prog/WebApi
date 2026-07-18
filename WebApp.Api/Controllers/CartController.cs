using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Api.DTOs;
using WebApp.Api.Services.Interfaces;

namespace WebApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart(AddToCartDto dto)
        {
            var userId = GetCurrentUserId();

            var cart = await _cartService.AddToCartAsync(userId, dto);

            return Ok(cart);
        }

        [HttpPut("items/{productId}")]
        public async Task<IActionResult> UpdateCartItem(int productId, UpdateCartItemDto dto)
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.UpdateCartItemAsync(userId, productId, dto);

            if (cart == null)
            {
                return NotFound();
            }

            return Ok(cart);
        }

        [HttpDelete("items/{productId}")]
        public async Task<IActionResult> RemoveFromCartAsync(int productId)
        {
            var userId = GetCurrentUserId();
            var removed = await _cartService.RemoveFromCartAsync(userId, productId);

            if(!removed) 
            {
                return NotFound();
            }
            return NoContent();
        }

        private int GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userId!);
        }
    }
}
