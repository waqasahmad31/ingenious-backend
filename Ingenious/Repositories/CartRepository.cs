using DAS.DataAccess.Helpers;
using Ingenious.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace Ingenious.Repositories
{
    public interface ICartRepository
    {
        Task<IEnumerable<GetCartDto>> GetCartByUserIdAsync(string aspNetUserId);
        Task<int> AddToCartAsync(CreateCartDto dto);
        Task<int> UpdateCartItemAsync(UpdateCartDto dto);
        Task<int> RemoveFromCartAsync(int cartId);
        Task<int> ClearCartByUserIdAsync(string aspNetUserId);
    }
    public class CartRepository : ICartRepository
    {
        private readonly ConnectionStrings _connectionStrings;

        public CartRepository(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public async Task<IEnumerable<GetCartDto>> GetCartByUserIdAsync(string aspNetUserId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_AspNetUserId", aspNetUserId)
            };

            var dataset = await DbHelper.GetDataSet("Cart_GetCartByUserId", parameters, _connectionStrings);

            var carts = new List<GetCartDto>();

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                var cart = new GetCartDto
                {
                    CartId = Convert.ToInt32(row["CartId"]),
                    AspNetUserId = Convert.ToString(row["AspNetUserId"]),
                    ProductId = Convert.ToInt32(row["ProductId"]),
                    VariationId = row["VariationId"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["VariationId"]),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    Price = Convert.ToDecimal(row["Price"]),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                    UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),

                    ProductName = row["ProductName"] == DBNull.Value ? null : Convert.ToString(row["ProductName"]),
                    VariationName = row["VariationName"] == DBNull.Value ? null : Convert.ToString(row["VariationName"]),
                    ProductImageUrl = row["ProductImageUrl"] == DBNull.Value ? null : Convert.ToString(row["ProductImageUrl"])
                };

                carts.Add(cart);
            }

            return carts;
        }

        public async Task<int> AddToCartAsync(CreateCartDto dto)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_AspNetUserId", dto.AspNetUserId),
                new MySqlParameter("@p_ProductId", dto.ProductId),
                new MySqlParameter("@p_VariationId", dto.VariationId),
                new MySqlParameter("@p_Quantity", dto.Quantity),
                new MySqlParameter("@p_Price", dto.Price)
            };

            return await DbHelper.ExecuteNonQuery("Cart_AddToCart", parameters, _connectionStrings);
        }

        public async Task<int> UpdateCartItemAsync(UpdateCartDto dto)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_CartId", dto.CartId),
                new MySqlParameter("@p_AspNetUserId", dto.AspNetUserId),
                new MySqlParameter("@p_ProductId", dto.ProductId),
                new MySqlParameter("@p_VariationId", dto.VariationId),
                new MySqlParameter("@p_Quantity", dto.Quantity),
                new MySqlParameter("@p_Price", dto.Price)
            };

            return await DbHelper.ExecuteNonQuery("Cart_UpdateCartItem", parameters, _connectionStrings);
        }

        public async Task<int> RemoveFromCartAsync(int cartId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_CartId", cartId)
            };

            return await DbHelper.ExecuteNonQuery("Cart_RemoveFromCart", parameters, _connectionStrings);
        }

        public async Task<int> ClearCartByUserIdAsync(string aspNetUserId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_AspNetUserId", aspNetUserId)
            };

            return await DbHelper.ExecuteNonQuery("Cart_ClearCartByUserId", parameters, _connectionStrings);
        }
    }
}
