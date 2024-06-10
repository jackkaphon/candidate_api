using EcommerceApi.Dto.CartDto;
using EcommerceApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IProductService _productService;

        public UserController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("{cartId}/addItem")]
        public async Task<IActionResult> AddItemToCart(int cartId, [FromBody] CartItemDto cartItemDto)
        {
            var cartItem = await _productService.AddItemToCart(cartId, cartItemDto);

            if (cartItem == null)
            {
                return BadRequest("Unable to add item to cart.");
            }

            return Ok(cartItem);
        }

        [HttpDelete("cartItem/{cartItemId}")]
        public async Task<IActionResult> RemoveItemFromCart(int cartItemId)
        {
            await _productService.RemoveItemFromCart(cartItemId);

            return NoContent();
        }
    }
}
