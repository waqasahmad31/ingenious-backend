using Ingenious.Models.Helpers;
using Ingenious.Models;
using Microsoft.AspNetCore.Mvc;
using Ingenious.Repositories;

namespace Ingenious.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartsController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        [HttpGet("GetCart/{aspNetUserId}")]
        public async Task<IActionResult> GetCart(string aspNetUserId)
        {
            try
            {
                var cart = await _cartRepository.GetCartByUserIdAsync(aspNetUserId);
                if (cart == null || !cart.Any())
                {
                    return NotFound(new ApiResponse<string>("No cart items found for the user.", 404));
                }
                return Ok(new ApiResponse<IEnumerable<GetCartDto>>(cart, "Cart items retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>("An error occurred while retrieving the cart: " + ex.Message, 500));
            }
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] CreateCartDto createCartDto)
        {
            try
            {
                if (createCartDto == null)
                {
                    return BadRequest(new ApiResponse<string>("Invalid cart item data.", 400));
                }

                var cartItemId = await _cartRepository.AddToCartAsync(createCartDto);
                var cartData = await _cartRepository.GetCartByUserIdAsync(createCartDto.AspNetUserId);
                return Ok(new ApiResponse<IEnumerable<GetCartDto>>(cartData, "Item added to cart successfully.", 202));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>("An error occurred while adding the item to the cart: " + ex.Message, 500));
            }
        }

        [HttpPut("UpdateCartItem")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartDto cartDto)
        {
            try
            {
                if (cartDto == null || cartDto.CartId <= 0)
                {
                    return BadRequest(new ApiResponse<string>("Invalid cart item data.", 400));
                }

                var isUpdated = await _cartRepository.UpdateCartItemAsync(cartDto);

                var cartData = await _cartRepository.GetCartByUserIdAsync(cartDto.AspNetUserId);

                return Ok(new ApiResponse<IEnumerable<GetCartDto>>(cartData, "Cart item updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>("An error occurred while updating the cart item: " + ex.Message, 500));
            }
        }

        [HttpDelete("RemoveFromCart/{cartId}/{aspNetUserId}")]
        public async Task<IActionResult> RemoveFromCart(int cartId, string aspNetUserId)
        {
            try
            {
                var isRemoved = await _cartRepository.RemoveFromCartAsync(cartId);

                var cartData = await _cartRepository.GetCartByUserIdAsync(aspNetUserId);

                return Ok(new ApiResponse<IEnumerable<GetCartDto>>(cartData, "Item removed from cart successfully.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>("An error occurred while removing the item from the cart: " + ex.Message, 500));
            }
        }

        [HttpDelete("ClearCart/{aspNetUserId}")]
        public async Task<IActionResult> ClearCart(string aspNetUserId)
        {
            try
            {
                var isCleared = await _cartRepository.ClearCartByUserIdAsync(aspNetUserId);

                var cartData = await _cartRepository.GetCartByUserIdAsync(aspNetUserId);

                return Ok(new ApiResponse<IEnumerable<GetCartDto>>(cartData,"Cart cleared successfully.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>("An error occurred while clearing the cart: " + ex.Message, 500));
            }
        }
    }
}
