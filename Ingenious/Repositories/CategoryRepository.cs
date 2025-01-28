using DAS.DataAccess.Helpers;
using Ingenious.Models;
using MySql.Data.MySqlClient;

namespace Ingenious.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int? categoryId = null);
        Task<int> UpsertCategoryAsync(UpsertCategoryDto dto);
        Task<int> DeleteCategoryAsync(int id);
    }
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ConnectionStrings _connectionStrings;

        public CategoryRepository(ConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(int? categoryId = null)
        {
            var parameters = new MySqlParameter[]
            {
                new MySqlParameter("@p_categoryId", categoryId ?? (object)DBNull.Value)
            };
            var categories = await DbHelper.GetList<CategoryDto>("GetCategories", parameters, _connectionStrings);

            return categories;
        }

        public async Task<int> UpsertCategoryAsync(UpsertCategoryDto dto)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_categoryId", dto.CategoryId),
                new MySqlParameter("@p_name", dto.Name),
                new MySqlParameter("@p_slug", dto.Slug),
                new MySqlParameter("@p_parentCategoryId", dto.ParentCategoryId),
                new MySqlParameter("@p_imageUrl", dto.ImageUrl),
                new MySqlParameter("@p_isActive", dto.IsActive)
            };

            return await DbHelper.ExecuteNonQuery("UpsertCategory", parameters, _connectionStrings);
        }

        public async Task<int> DeleteCategoryAsync(int categoryId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_categoryId", categoryId)
            };

            return await DbHelper.ExecuteNonQuery("SoftDeleteCategory", parameters, _connectionStrings);
        }
    }
}
