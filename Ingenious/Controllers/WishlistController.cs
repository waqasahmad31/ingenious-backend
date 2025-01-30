using Ingenious.Models.Helpers;
using Ingenious.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ingenious.Repositories;

namespace Ingenious.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistController(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        [HttpGet("GetWishlistByUserId/{aspNetUserId}")]
        public async Task<IActionResult> GetWishlistByUserId(string aspNetUserId)
        {
            try
            {
                var wishlist = await _wishlistRepository.GetWishlistByUserIdAsync(aspNetUserId);
                return Ok(new ApiResponse<IEnumerable<GetWishlistDto>>(wishlist, "Wishlist retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while retrieving wishlist: {ex.Message}", 500));
            }
        }

        [HttpPost("AddToWishlist")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddWishlistDto wishlistDto)
        {
            try
            {
                if (wishlistDto == null)
                {
                    return BadRequest(new ApiResponse<string>("Invalid wishlist data.", 400));
                }

                var wishlistId = await _wishlistRepository.AddToWishlistAsync(wishlistDto);
                return CreatedAtAction(nameof(GetWishlistByUserId), new { wishlistId }, new ApiResponse<int>(wishlistId, "Product added to wishlist.", 201));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while adding to wishlist: {ex.Message}", 500));
            }
        }

        [HttpDelete("RemoveFromWishlist/{wishlistId}")]
        public async Task<IActionResult> RemoveFromWishlist(int wishlistId)
        {
            try
            {
                if (wishlistId <= 0)
                {
                    return BadRequest(new ApiResponse<string>("Invalid wishlist ID.", 400));
                }

                var result = await _wishlistRepository.RemoveFromWishlistAsync(wishlistId);
                if (result <= 0)
                {
                    return NotFound(new ApiResponse<string>("Wishlist item not found.", 404));
                }

                return Ok(new ApiResponse<string>("Product removed from wishlist.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while removing from wishlist: {ex.Message}", 500));
            }
        }
    }
}
