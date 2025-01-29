using Ingenious.Models.Helpers;
using Ingenious.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ingenious.Repositories;
using Mysqlx.Crud;

namespace Ingenious.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetOrders(int? orderId = null)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersAsync(orderId: orderId);
                return Ok(new ApiResponse<IEnumerable<GetOrderDto>>(orders, "Orders retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while retrieving orders: {ex.Message}", 500));
            }
        }

        [HttpPost("AddOrder")]
        public async Task<IActionResult> AddOrder([FromBody] AddOrderDto orderDto)
        {
            try
            {
                if (orderDto == null || orderDto.TotalAmount <= 0)
                {
                    return BadRequest(new ApiResponse<string>("Invalid order data.", 400));
                }

                var orderId = await _orderRepository.AddOrderAsync(orderDto);
                var orders = await _orderRepository.GetOrdersAsync(orderId: orderId);
                var order = orders.FirstOrDefault();
                return Ok(new ApiResponse<GetOrderDto>(order, "Order placed successfully.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while placing an order: {ex.Message}", 500));
            }
        }
    }
}
