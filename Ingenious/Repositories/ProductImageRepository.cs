using DAS.DataAccess.Helpers;
using Ingenious.Models;
using MySql.Data.MySqlClient;

namespace Ingenious.Repositories
{
    public interface IProductImageRepository
    {
        Task<IEnumerable<ProductImageDto>> GetImagesByProductIdAsync(int productId);
        Task<int> AddImageAsync(CreateProductImageDto image);
        Task<bool> DeleteImageAsync(int imageId);
    }
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly ConnectionStrings _connectionStrings;

        public ProductImageRepository(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public async Task<IEnumerable<ProductImageDto>> GetImagesByProductIdAsync(int productId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_productId", productId)
            };

            var images = await DbHelper.GetList<ProductImageDto>("GetProductImagesByProductId",parameters,_connectionStrings);

            return images;
        }

        public async Task<int> AddImageAsync(CreateProductImageDto image)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_productId", image.ProductId),
                new MySqlParameter("@p_imageUrl", image.ImageUrl),
                new MySqlParameter("@p_isDefault", image.IsDefault)
            };

            return await DbHelper.ExecuteNonQuery("AddProductImage", parameters, _connectionStrings);
        }

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_imageId", imageId)
            };

            return await DbHelper.ExecuteNonQuery("DeleteProductImage", parameters, _connectionStrings) > 0;
        }
    }
}
