//using Ingenious.Models.Helpers;
//using Ingenious.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Ingenious.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class PaymentsController : ControllerBase
//    {
//        private readonly IPaymentRepository _paymentRepository;

//        public PaymentsController(IPaymentRepository paymentRepository)
//        {
//            _paymentRepository = paymentRepository;
//        }

//        [HttpGet("GetPayments")]
//        public async Task<IActionResult> GetPayments()
//        {
//            try
//            {
//                var payments = await _paymentRepository.GetPaymentsAsync();
//                return Ok(new ApiResponse<IEnumerable<PaymentDto>>(payments, "Payments retrieved successfully."));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while retrieving payments: {ex.Message}", 500));
//            }
//        }

//        [HttpPost("AddPayment")]
//        public async Task<IActionResult> AddPayment([FromBody] PaymentDto paymentDto)
//        {
//            try
//            {
//                if (paymentDto == null)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid payment data.", 400));
//                }

//                var id = await _paymentRepository.AddPaymentAsync(paymentDto);
//                return CreatedAtAction(nameof(GetPayments), new { id }, new ApiResponse<int>(id, "Payment added successfully.", 201));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while adding the payment: {ex.Message}", 500));
//            }
//        }

//        [HttpPut("UpdatePayment")]
//        public async Task<IActionResult> UpdatePayment([FromBody] PaymentDto paymentDto)
//        {
//            try
//            {
//                if (paymentDto == null || paymentDto.PaymentId <= 0)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid payment data.", 400));
//                }

//                await _paymentRepository.UpdatePaymentAsync(paymentDto);
//                return Ok(new ApiResponse<string>("Payment updated successfully.", 200));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while updating the payment: {ex.Message}", 500));
//            }
//        }

//        [HttpDelete("DeletePayment/{id}")]
//        public async Task<IActionResult> DeletePayment(int id)
//        {
//            try
//            {
//                if (id <= 0)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid payment ID.", 400));
//                }

//                await _paymentRepository.DeletePaymentAsync(id);
//                return Ok(new ApiResponse<string>("Payment deleted successfully.", 200));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while deleting the payment: {ex.Message}", 500));
//            }
//        }
//    }
//}
