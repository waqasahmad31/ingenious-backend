//using Ingenious.Models.Helpers;
//using Ingenious.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Ingenious.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class OrderItemsController : ControllerBase
//    {
//        private readonly IOrderItemRepository _orderItemRepository;

//        public OrderItemsController(IOrderItemRepository orderItemRepository)
//        {
//            _orderItemRepository = orderItemRepository;
//        }

//        // Get all items in an order
//        [HttpGet("GetOrderItemsForOrder/{orderId}")]
//        public async Task<IActionResult> GetOrderItemsForOrder(int orderId)
//        {
//            try
//            {
//                var orderItems = await _orderItemRepository.GetOrderItemsByOrderIdAsync(orderId);
//                return Ok(new ApiResponse<IEnumerable<OrderItemDto>>(orderItems, "Order items retrieved successfully."));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while retrieving order items: {ex.Message}", 500));
//            }
//        }

//        // Add an item to an order
//        [HttpPost("AddItemToOrder")]
//        public async Task<IActionResult> AddItemToOrder([FromBody] CreateOrderItemDto orderItemDto)
//        {
//            try
//            {
//                if (orderItemDto == null)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid order item data.", 400));
//                }

//                var id = await _orderItemRepository.AddItemToOrderAsync(orderItemDto);
//                return CreatedAtAction(nameof(GetOrderItemsForOrder), new { id }, new ApiResponse<int>(id, "Order item added successfully.", 201));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while adding the item to the order: {ex.Message}", 500));
//            }
//        }

//        // Update an order item (e.g., quantity or price)
//        [HttpPut("UpdateOrderItem")]
//        public async Task<IActionResult> UpdateOrderItem([FromBody] UpdateOrderItemDto orderItemDto)
//        {
//            try
//            {
//                if (orderItemDto == null || orderItemDto.OrderItemId <= 0)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid order item data.", 400));
//                }

//                await _orderItemRepository.UpdateOrderItemAsync(orderItemDto);
//                return Ok(new ApiResponse<string>("Order item updated successfully.", 200));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while updating the order item: {ex.Message}", 500));
//            }
//        }

//        // Remove an item from an order
//        [HttpDelete("RemoveItemFromOrder/{id}")]
//        public async Task<IActionResult> RemoveItemFromOrder(int id)
//        {
//            try
//            {
//                if (id <= 0)
//                {
//                    return BadRequest(new ApiResponse<string>("Invalid order item ID.", 400));
//                }

//                await _orderItemRepository.RemoveItemFromOrderAsync(id);
//                return Ok(new ApiResponse<string>("Order item removed successfully.", 200));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<string>($"An error occurred while removing the order item: {ex.Message}", 500));
//            }
//        }
//    }
//}
