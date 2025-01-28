//using Ingenious.Models.Helpers;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Ingenious.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class WishlistController : ControllerBase
//    {
//        private readonly IWishlistRepository _wishlistRepository;

//        public WishlistController(IWishlistRepository wishlistRepository)
//        {
//            _wishlistRepository = wishlistRepository;
//        }

//        // Get all items in a customer's wishlist
//        [HttpGet("GetWishlistForCustomer/{customerId}")]
//        public async Task<IActionResult> GetWishlistForCustomer(int customerId)
//        {
//            try
//            {
//                var wishlistItems = await _wishlistRepository.GetWishlistItemsByCustomerIdAsync(customerId);
//                return Ok(new ApiResponse<IEnumerable<WishlistItemDto>>(wishlistItems, "Wishlist items retrieved successfully."));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while retrieving wishlist items: {ex.Message}", 500));
//            }
//        }

//        // Add an item to the wishlist
//        [HttpPost("AddItemToWishlist")]
//        public async Task<IActionResult> AddItemToWishlist([FromBody] CreateWishlistItemDto wishlistItemDto)
//        {
//            try
//            {
//                if (wishlistItemDto == null)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid wishlist item data.", 400));
//                }

//                var id = await _wishlistRepository.AddItemToWishlistAsync(wishlistItemDto);
//                return CreatedAtAction(nameof(GetWishlistForCustomer), new { id }, new ApiResponse<int>(id, "Wishlist item added successfully.", 201));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while adding the item to the wishlist: {ex.Message}", 500));
//            }
//        }

//        // Remove an item from the wishlist
//        [HttpDelete("RemoveItemFromWishlist/{id}")]
//        public async Task<IActionResult> RemoveItemFromWishlist(int id)
//        {
//            try
//            {
//                if (id <= 0)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid wishlist item ID.", 400));
//                }

//                await _wishlistRepository.RemoveItemFromWishlistAsync(id);
//                return Ok(new ApiResponse<string>("Wishlist item removed successfully.", 200));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while removing the item from the wishlist: {ex.Message}", 500));
//            }
//        }
//    }
//}
