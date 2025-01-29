using DAS.DataAccess.Helpers;
using Ingenious.Models;
using MySql.Data.MySqlClient;

namespace Ingenious.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<GetOrderDto>> GetOrdersAsync(int? orderId = null);
        Task<int> AddOrderAsync(AddOrderDto saleDto);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly IProductRepository _productRepository;
        private readonly ConnectionStrings _connectionStrings;

        public OrderRepository(
            IProductRepository productRepository,
            ConnectionStrings connectionStrings)
        {
            _productRepository = productRepository;
            _connectionStrings = connectionStrings;
        }

        public async Task<int> AddOrderAsync(AddOrderDto dto)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_AspNetUserId", dto.AspNetUserId),
                new MySqlParameter("@p_TotalAmount", dto.TotalAmount),
                new MySqlParameter("@p_OrderStatus", dto.OrderStatus.ToString()),
                new MySqlParameter("@p_PaymentStatus", dto.PaymentStatus.ToString()),
                new MySqlParameter("@p_ShippingAddressId", dto.ShippingAddressId)
            };

            //var orderId = await DbHelper.ExecuteNonQuery("Orders_AddOrder", parameters, _connectionStrings);

            //foreach (var detail in dto.OrderItems)
            //{
            //    var detailParams = new[]
            //    {
            //        new MySqlParameter("@p_OrderId", orderId),
            //        new MySqlParameter("@p_ProductId", detail.ProductId),
            //        new MySqlParameter("@p_VariationId", detail.VariationId),
            //        new MySqlParameter("@p_Quantity", detail.Quantity),
            //        new MySqlParameter("@p_Price", detail.Price)
            //    };

            //    await DbHelper.ExecuteNonQuery("Orders_AddOrderItems", detailParams, _connectionStrings);

            //    var product = await _productRepository.GetProductByIdAsync(detail.ProductId);
            //    var updateProduct = new CreateProductDto()
            //    {
            //        CategoryId = product.CategoryId,
            //        Description = product.Description,
            //        Discount = product.Discount,
            //        Name = product.Name,
            //        Price = product.Price,
            //        Slug = product.Slug,
            //        Stock = product.Stock - detail.Quantity,
            //    };

            //    var result = _productRepository.UpdateProductAsync(detail.ProductId, updateProduct);
            //}

            //return orderId;
            return 4;
        }

        public async Task<IEnumerable<GetOrderDto>> GetOrdersAsync(int? orderId = null)
        {
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_OrderId", orderId ?? (object)DBNull.Value),
            };
            var orders = await DbHelper.GetList<GetOrderDto>("Orders_GetOrders", parameters, _connectionStrings);

            foreach (var order in orders)
            {
                var detailParameters = new MySqlParameter[]
                {
                    new MySqlParameter("@p_OrderId", order.OrderId)
                };

                var orderItems = await DbHelper.GetList<GetOrderItemsDto>("Orders_GetOrderItemsByOrderId", detailParameters, _connectionStrings);
                order.OrderItems = orderItems;
            }

            return orders;
        }
    }
}
