//using Ingenious.Models.Helpers;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Ingenious.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ShippingsController : ControllerBase
//    {
//        private readonly IShippingRepository _shippingRepository;

//        public ShippingsController(IShippingRepository shippingRepository)
//        {
//            _shippingRepository = shippingRepository;
//        }

//        [HttpGet("GetShippings")]
//        public async Task<IActionResult> GetShippings()
//        {
//            try
//            {
//                var shippings = await _shippingRepository.GetShippingsAsync();
//                return Ok(new ApiResponse<IEnumerable<ShippingDto>>(shippings, "Shippings retrieved successfully."));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while retrieving shippings: {ex.Message}", 500));
//            }
//        }

//        [HttpPost("AddShipping")]
//        public async Task<IActionResult> AddShipping([FromBody] ShippingDto shippingDto)
//        {
//            try
//            {
//                if (shippingDto == null)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid shipping data.", 400));
//                }

//                var id = await _shippingRepository.AddShippingAsync(shippingDto);
//                return CreatedAtAction(nameof(GetShippings), new { id }, new ApiResponse<int>(id, "Shipping added successfully.", 201));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while adding the shipping: {ex.Message}", 500));
//            }
//        }

//        [HttpPut("UpdateShipping")]
//        public async Task<IActionResult> UpdateShipping([FromBody] ShippingDto shippingDto)
//        {
//            try
//            {
//                if (shippingDto == null || shippingDto.ShippingId <= 0)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid shipping data.", 400));
//                }

//                await _shippingRepository.UpdateShippingAsync(shippingDto);
//                return Ok(new ApiResponse<string>("Shipping updated successfully.", 200));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while updating the shipping: {ex.Message}", 500));
//            }
//        }

//        [HttpDelete("DeleteShipping/{id}")]
//        public async Task<IActionResult> DeleteShipping(int id)
//        {
//            try
//            {
//                if (id <= 0)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid shipping ID.", 400));
//                }

//                await _shippingRepository.DeleteShippingAsync(id);
//                return Ok(new ApiResponse<string>("Shipping deleted successfully.", 200));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while deleting the shipping: {ex.Message}", 500));
//            }
//        }
//    }
//}
