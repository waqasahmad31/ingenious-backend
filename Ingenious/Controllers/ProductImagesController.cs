using Ingenious.Models.Helpers;
using Ingenious.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ingenious.Repositories;

namespace Ingenious.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageRepository _productImageRepository;

        public ProductImageController(IProductImageRepository productImageRepository)
        {
            _productImageRepository = productImageRepository;
        }

        [HttpGet("GetImagesByProductId/{productId}")]
        public async Task<IActionResult> GetImagesByProductId(int productId)
        {
            try
            {
                var images = await _productImageRepository.GetImagesByProductIdAsync(productId);
                if (images == null || !images.Any())
                    return NotFound(new ApiResponse<string>("No images found for this product.", 404));

                return Ok(new ApiResponse<IEnumerable<ProductImageDto>>(images, "Images retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred: {ex.Message}", 500));
            }
        }

        [HttpPost("AddImage")]
        public async Task<IActionResult> AddImage([FromBody] CreateProductImageDto imageDto)
        {
            try
            {
                await _productImageRepository.AddImageAsync(imageDto);
                return Ok(new ApiResponse<CreateProductImageDto>(imageDto, "Image added successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred: {ex.Message}", 500));
            }
        }

        [HttpDelete("DeleteImage/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                var success = await _productImageRepository.DeleteImageAsync(id);
                if (!success)
                    return NotFound(new ApiResponse<string>($"Image with ID {id} not found.", 404));

                return Ok(new ApiResponse<string>("Image deleted successfully.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred: {ex.Message}", 500));
            }
        }
    }
}
