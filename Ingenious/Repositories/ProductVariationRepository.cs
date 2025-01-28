using DAS.DataAccess.Helpers;
using Ingenious.Models;
using MySql.Data.MySqlClient;

namespace Ingenious.Repositories
{
    public interface IProductVariationRepository
    {
        Task<IEnumerable<ProductVariationDto>> GetVariationsByProductIdAsync(int productId);
        Task<int> AddVariationAsync(CreateProductVariationDto variation);
        Task<bool> UpdateVariationAsync(int variationId, CreateProductVariationDto variation);
        Task<bool> DeleteVariationAsync(int variationId);
    }
    public class ProductVariationRepository : IProductVariationRepository
    {
        private readonly ConnectionStrings _connectionStrings;

        public ProductVariationRepository(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public async Task<IEnumerable<ProductVariationDto>> GetVariationsByProductIdAsync(int productId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_productId", productId)
            };

            var variations = await DbHelper.GetList<ProductVariationDto>("GetProductVariationsByProductId",parameters,_connectionStrings);

            return variations;
        }

        public async Task<int> AddVariationAsync(CreateProductVariationDto variation)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_productId", variation.ProductId),
                new MySqlParameter("@p_variationName", variation.VariationName),
                new MySqlParameter("@p_price", variation.Price ?? (object)DBNull.Value),
                new MySqlParameter("@p_stock", variation.Stock)
            };

            return await DbHelper.ExecuteNonQuery("AddProductVariation", parameters, _connectionStrings);
        }

        public async Task<bool> UpdateVariationAsync(int variationId, CreateProductVariationDto variation)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_variationId", variationId),
                new MySqlParameter("@p_variationName", variation.VariationName),
                new MySqlParameter("@p_price", variation.Price ?? (object)DBNull.Value),
                new MySqlParameter("@p_stock", variation.Stock)
            };

            return await DbHelper.ExecuteNonQuery("UpdateProductVariation", parameters, _connectionStrings) > 0;
        }

        public async Task<bool> DeleteVariationAsync(int variationId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_variationId", variationId)
            };

            return await DbHelper.ExecuteNonQuery("DeleteProductVariation", parameters, _connectionStrings) > 0;
        }
    }

}
