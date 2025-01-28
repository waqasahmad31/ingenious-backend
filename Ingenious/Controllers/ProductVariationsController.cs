using Ingenious.Models.Helpers;
using Ingenious.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ingenious.Repositories;

namespace Ingenious.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariationController : ControllerBase
    {
        private readonly IProductVariationRepository _productVariationRepository;

        public ProductVariationController(IProductVariationRepository productVariationRepository)
        {
            _productVariationRepository = productVariationRepository;
        }

        [HttpGet("GetVariationsByProductId/{productId}")]
        public async Task<IActionResult> GetVariationsByProductId(int productId)
        {
            try
            {
                var variations = await _productVariationRepository.GetVariationsByProductIdAsync(productId);
                if (variations == null || !variations.Any())
                    return NotFound(new ApiResponse<string>("No variations found for this product.", 404));

                return Ok(new ApiResponse<IEnumerable<ProductVariationDto>>(variations, "Variations retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred: {ex.Message}", 500));
            }
        }

        [HttpPost("AddVariation")]
        public async Task<IActionResult> AddVariation([FromBody] CreateProductVariationDto variationDto)
        {
            try
            {
                await _productVariationRepository.AddVariationAsync(variationDto);
                return Ok(new ApiResponse<CreateProductVariationDto>(variationDto, "Variation added successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred: {ex.Message}", 500));
            }
        }

        [HttpDelete("DeleteVariation/{id}")]
        public async Task<IActionResult> DeleteVariation(int id)
        {
            try
            {
                var success = await _productVariationRepository.DeleteVariationAsync(id);
                if (!success)
                    return NotFound(new ApiResponse<string>($"Variation with ID {id} not found.", 404));

                return Ok(new ApiResponse<string>("Variation deleted successfully.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred: {ex.Message}", 500));
            }
        }
    }
}
