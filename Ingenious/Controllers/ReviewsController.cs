//using Ingenious.Models.Helpers;
//using Ingenious.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Ingenious.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ReviewsController : ControllerBase
//    {
//        private readonly IReviewRepository _reviewRepository;

//        public ReviewsController(IReviewRepository reviewRepository)
//        {
//            _reviewRepository = reviewRepository;
//        }

//        [HttpGet("GetReviews")]
//        public async Task<IActionResult> GetReviews()
//        {
//            try
//            {
//                var reviews = await _reviewRepository.GetReviewsAsync();
//                return Ok(new ApiResponse<IEnumerable<ReviewDto>>(reviews, "Reviews retrieved successfully."));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while retrieving reviews: {ex.Message}", 500));
//            }
//        }

//        [HttpPost("AddReview")]
//        public async Task<IActionResult> AddReview([FromBody] ReviewDto reviewDto)
//        {
//            try
//            {
//                if (reviewDto == null)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid review data.", 400));
//                }

//                var id = await _reviewRepository.AddReviewAsync(reviewDto);
//                return CreatedAtAction(nameof(GetReviews), new { id }, new ApiResponse<int>(id, "Review added successfully.", 201));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while adding the review: {ex.Message}", 500));
//            }
//        }

//        [HttpPut("UpdateReview")]
//        public async Task<IActionResult> UpdateReview([FromBody] ReviewDto reviewDto)
//        {
//            try
//            {
//                if (reviewDto == null || reviewDto.ReviewId <= 0)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid review data.", 400));
//                }

//                await _reviewRepository.UpdateReviewAsync(reviewDto);
//                return Ok(new ApiResponse<string>("Review updated successfully.", 200));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while updating the review: {ex.Message}", 500));
//            }
//        }

//        [HttpDelete("DeleteReview/{id}")]
//        public async Task<IActionResult> DeleteReview(int id)
//        {
//            try
//            {
//                if (id <= 0)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid review ID.", 400));
//                }

//                await _reviewRepository.DeleteReviewAsync(id);
//                return Ok(new ApiResponse<string>("Review deleted successfully.", 200));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while deleting the review: {ex.Message}", 500));
//            }
//        }
//    }
//}
