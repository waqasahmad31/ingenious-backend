//using Ingenious.Models.Helpers;
//using Ingenious.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Ingenious.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class OrdersController : ControllerBase
//    {
//        private readonly IOrderRepository _orderRepository;

//        public OrdersController(IOrderRepository orderRepository)
//        {
//            _orderRepository = orderRepository;
//        }

//        [HttpGet("GetOrders")]
//        public async Task<IActionResult> GetOrders()
//        {
//            try
//            {
//                var orders = await _orderRepository.GetOrdersAsync();
//                return Ok(new ApiResponse<IEnumerable<OrderDto>>(orders, "Orders retrieved successfully."));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while retrieving orders: {ex.Message}", 500));
//            }
//        }

//        [HttpPost("AddOrder")]
//        public async Task<IActionResult> AddOrder([FromBody] OrderDto orderDto)
//        {
//            try
//            {
//                if (orderDto == null)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid order data.", 400));
//                }

//                var id = await _orderRepository.AddOrderAsync(orderDto);
//                return CreatedAtAction(nameof(GetOrders), new { id }, new ApiResponse<int>(id, "Order added successfully.", 201));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while adding the order: {ex.Message}", 500));
//            }
//        }

//        [HttpPut("UpdateOrder")]
//        public async Task<IActionResult> UpdateOrder([FromBody] OrderDto orderDto)
//        {
//            try
//            {
//                if (orderDto == null || orderDto.OrderId <= 0)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid order data.", 400));
//                }

//                await _orderRepository.UpdateOrderAsync(orderDto);
//                return Ok(new ApiResponse<string>("Order updated successfully.", 200));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while updating the order: {ex.Message}", 500));
//            }
//        }

//        [HttpDelete("DeleteOrder/{id}")]
//        public async Task<IActionResult> DeleteOrder(int id)
//        {
//            try
//            {
//                if (id <= 0)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid order ID.", 400));
//                }

//                await _orderRepository.DeleteOrderAsync(id);
//                return Ok(new ApiResponse<string>("Order deleted successfully.", 200));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while deleting the order: {ex.Message}", 500));
//            }
//        }
//    }
//}
