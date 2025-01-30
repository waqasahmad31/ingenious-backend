using Ingenious.Models.Helpers;
using Ingenious.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Ingenious.Repositories;

namespace Ingenious.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }


        [HttpPost("AddCompleteProduct")]
        public async Task<IActionResult> AddCompleteProduct([FromBody] ProductCreateCombinedDto productDto)
        {
            try
            {
                var result = await _productRepository.AddProductWithDetailsAsync(productDto);
                if (result)
                {
                    return Ok(new ApiResponse<bool>(result, "Product created successfully."));
                }
                return BadRequest(new ApiResponse<bool>(result, "Product not created.", 400));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while adding the product: {ex.Message}", 500));
            }
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts(
            int? productId = null,
            int? categoryId = null,
            string? name = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minStock = null,
            int? maxStock = null,
            bool? isActive = true)
        {
            try
            {
                var products = await _productRepository.GetAllProductsAsync(productId, categoryId, name, minPrice, maxPrice, minStock, maxStock, isActive);
                if (products == null || !products.Any())
                    return NotFound(new ApiResponse<string>("No products found.", 404));

                return Ok(new ApiResponse<IEnumerable<GetProductDto>>(products, "Products retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred: {ex.Message}", 500));
            }
        }

        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound(new ApiResponse<string>("Product not found.", 404));

                return Ok(new ApiResponse<GetProductDto>(product, "Product retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred: {ex.Message}", 500));
            }
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            try
            {
                if (productDto == null)
                    return BadRequest(new ApiResponse<string>("Invalid product data.", 400));

                await _productRepository.AddProductAsync(productDto);
                return Ok(new ApiResponse<CreateProductDto>(productDto, "Product created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred: {ex.Message}", 500));
            }
        }

        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] CreateProductDto productDto)
        {
            try
            {
                await _productRepository.UpdateProductAsync(id, productDto);
                return Ok(new ApiResponse<CreateProductDto>(productDto, "Product updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred: {ex.Message}", 500));
            }
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var success = await _productRepository.DeleteProductAsync(id);
                if (!success)
                    return NotFound(new ApiResponse<string>($"Product with ID {id} not found.", 404));

                return Ok(new ApiResponse<string>("Product deleted successfully.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred: {ex.Message}", 500));
            }
        }
    }
}
