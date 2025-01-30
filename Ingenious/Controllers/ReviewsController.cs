using Ingenious.Models.Helpers;
using Ingenious.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ingenious.Repositories;

namespace Ingenious.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewsController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [HttpGet("GetReviewsByProductId/{productId}")]
        public async Task<IActionResult> GetReviewsByProductId(int productId)
        {
            try
            {
                var reviews = await _reviewRepository.GetReviewsByProductIdAsync(productId);
                return Ok(new ApiResponse<IEnumerable<GetReviewDto>>(reviews, "Reviews retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while retrieving reviews: {ex.Message}", 500));
            }
        }

        [HttpGet("GetReviewById/{reviewId}")]
        public async Task<IActionResult> GetReviewById(int reviewId)
        {
            try
            {
                var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
                if (review == null)
                {
                    return NotFound(new ApiResponse<string>("Review not found.", 404));
                }

                return Ok(new ApiResponse<GetReviewDto>(review, "Review retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while retrieving the review: {ex.Message}", 500));
            }
        }

        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview([FromBody] AddReviewDto reviewDto)
        {
            try
            {
                if (reviewDto == null)
                {
                    return BadRequest(new ApiResponse<string>("Invalid review data.", 400));
                }

                var reviewId = await _reviewRepository.AddReviewAsync(reviewDto);
                return CreatedAtAction(nameof(GetReviewById), new { reviewId }, new ApiResponse<int>(reviewId, "Review added successfully.", 201));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while adding the review: {ex.Message}", 500));
            }
        }

        [HttpPut("UpdateReview")]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewDto reviewDto)
        {
            try
            {
                if (reviewDto == null || reviewDto.ReviewId <= 0)
                {
                    return BadRequest(new ApiResponse<string>("Invalid review data.", 400));
                }

                var result = await _reviewRepository.UpdateReviewAsync(reviewDto);
                
                return Ok(new ApiResponse<string>("Review updated successfully.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while updating the review: {ex.Message}", 500));
            }
        }

        [HttpDelete("DeleteReview/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                if (reviewId <= 0)
                {
                    return BadRequest(new ApiResponse<string>("Invalid review ID.", 400));
                }

                var result = await _reviewRepository.DeleteReviewAsync(reviewId);
                
                return Ok(new ApiResponse<string>("Review deleted successfully.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while deleting the review: {ex.Message}", 500));
            }
        }
    }
}
