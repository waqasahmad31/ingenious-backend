using DAS.DataAccess.Helpers;
using Ingenious.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Transactions;

namespace Ingenious.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<GetProductDto>> GetAllProductsAsync(
            int? productId = null,
            int? categoryId = null,
            string? name = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minStock = null,
            int? maxStock = null,
            bool? isActive = true);
        Task<GetProductDto> GetProductByIdAsync(int productId);
        Task<bool> AddProductWithDetailsAsync(ProductCreateCombinedDto productDto);
        Task<int> AddProductAsync(CreateProductDto product);
        Task<bool> UpdateProductAsync(int productId, CreateProductDto product);
        Task<bool> DeleteProductAsync(int productId);
    }
    public class ProductRepository : IProductRepository
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IProductVariationRepository _productVariationRepository;
        private readonly ConnectionStrings _connectionStrings;

        public ProductRepository(
            ConnectionStrings connectionStrings,
            IProductImageRepository productImageRepository,
            IProductVariationRepository productVariationRepository
        )
        {
            _productImageRepository = productImageRepository;
            _productVariationRepository = productVariationRepository;
            _connectionStrings = connectionStrings;
        }

        public async Task<IEnumerable<GetProductDto>> GetAllProductsAsync(
            int? productId = null,
            int? categoryId = null,
            string? name = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minStock = null,
            int? maxStock = null,
            bool? isActive = true)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_productId", productId ?? (object)DBNull.Value),
                new MySqlParameter("@p_categoryId", categoryId ?? (object)DBNull.Value),
                new MySqlParameter("@p_name", name ?? (object)DBNull.Value),
                new MySqlParameter("@p_minPrice", minPrice ?? (object)DBNull.Value),
                new MySqlParameter("@p_maxPrice", maxPrice ?? (object)DBNull.Value),
                new MySqlParameter("@p_minStock", minStock ?? (object)DBNull.Value),
                new MySqlParameter("@p_maxStock", maxStock ?? (object)DBNull.Value),
                new MySqlParameter("@p_isActive", isActive ?? (object)DBNull.Value),
            };

            var dataset = await DbHelper.GetDataSet("GetAllProducts", parameters, _connectionStrings);

            if (dataset.Tables.Count == 0 || dataset.Tables[0].Rows.Count == 0)
                return new List<GetProductDto>();

            var productTable = dataset.Tables[0];

            var products = productTable.AsEnumerable()
                .GroupBy(row => row.Field<int>("ProductId"))
                .Select(group =>
                {
                    var firstRow = group.First();

                    return new GetProductDto
                    {
                        ProductId = firstRow.Field<int>("ProductId"),
                        CategoryId = firstRow.Field<int>("CategoryId"),
                        Name = firstRow.Field<string>("Name"),
                        Slug = firstRow.Field<string>("Slug"),
                        Description = firstRow.Field<string>("Description"),
                        Price = firstRow.Field<decimal>("Price"),
                        Discount = firstRow.Field<decimal>("Discount"),
                        Stock = firstRow.Field<int>("Stock"),
                        IsActive = firstRow.Field<bool>("IsActive"),
                        CreatedAt = firstRow.Field<DateTime>("CreatedAt"),
                        UpdatedAt = firstRow.Field<DateTime>("UpdatedAt"),

                        Category = new CategoryDto
                        {
                            CategoryId = firstRow.Field<int>("Category_CategoryId"),
                            Name = firstRow.Field<string>("Category_Name"),
                            Slug = firstRow.Field<string>("Category_Slug"),
                            ParentCategoryId = firstRow.Field<int?>("Category_ParentCategoryId"),
                            ImageUrl = firstRow.Field<string>("Category_ImageUrl"),
                            IsActive = firstRow.Field<bool>("Category_IsActive"),
                            CreatedAt = firstRow.Field<DateTime>("Category_CreatedAt"),
                            UpdatedAt = firstRow.Field<DateTime>("Category_UpdatedAt")
                        },

                        ProductImages = group
                            .Where(row => row.Field<int?>("Image_ImageId") != null)
                            .Select(row => new ProductImageDto
                            {
                                ImageId = row.Field<int>("Image_ImageId"),
                                ProductId = row.Field<int>("Image_ProductId"),
                                ImageUrl = row.Field<string>("Image_ImageUrl"),
                                IsDefault = row.Field<bool>("Image_IsDefault"),
                                CreatedAt = row.Field<DateTime>("Image_CreatedAt"),
                                UpdatedAt = row.Field<DateTime>("Image_UpdatedAt")
                            })
                            .ToList(),

                        ProductVariations = group
                            .Where(row => row.Field<int?>("Variation_VariationId") != null)
                            .Select(row => new ProductVariationDto
                            {
                                VariationId = row.Field<int>("Variation_VariationId"),
                                ProductId = row.Field<int>("Variation_ProductId"),
                                VariationName = row.Field<string>("Variation_Name"),
                                Price = row.Field<decimal?>("Variation_Price"),
                                Stock = row.Field<int>("Variation_Stock"),
                                IsActive = row.Field<bool>("Variation_IsActive"),
                                CreatedAt = row.Field<DateTime>("Variation_CreatedAt"),
                                UpdatedAt = row.Field<DateTime>("Variation_UpdatedAt")
                            })
                            .ToList()
                    };
                })
                .ToList();

            return products;
        }

        public async Task<GetProductDto> GetProductByIdAsync(int productId)
        {
            var products = await GetAllProductsAsync(productId);

            var product = products.FirstOrDefault();

            return product;
        }

        public async Task<bool> AddProductWithDetailsAsync(ProductCreateCombinedDto productDto)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                // Add product
                var productId = await AddProductAsync(productDto.Product);

                // Add images
                foreach (var image in productDto.Images)
                {
                    image.ProductId = productId;
                    await _productImageRepository.AddImageAsync(image);
                }

                // Add variations
                foreach (var variation in productDto.Variations)
                {
                    variation.ProductId = productId;
                    await _productVariationRepository.AddVariationAsync(variation);
                }

                transaction.Complete();
                return true;
            }
            catch (Exception ex)
            {
                // Log error here
                return false;
            }
        }

        public async Task<int> AddProductAsync(CreateProductDto product)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_categoryId", product.CategoryId),
                new MySqlParameter("@p_name", product.Name),
                new MySqlParameter("@p_slug", product.Slug),
                new MySqlParameter("@p_description", product.Description ?? (object)DBNull.Value),
                new MySqlParameter("@p_price", product.Price),
                new MySqlParameter("@p_discount", product.Discount),
                new MySqlParameter("@p_stock", product.Stock)
            };

            return await DbHelper.ExecuteNonQuery("CreateProduct", parameters, _connectionStrings);
        }

        public async Task<bool> UpdateProductAsync(int productId, CreateProductDto product)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_productId", productId),
                new MySqlParameter("@p_categoryId", product.CategoryId),
                new MySqlParameter("@p_name", product.Name),
                new MySqlParameter("@p_slug", product.Slug),
                new MySqlParameter("@p_description", product.Description ?? (object)DBNull.Value),
                new MySqlParameter("@p_price", product.Price),
                new MySqlParameter("@p_discount", product.Discount),
                new MySqlParameter("@p_stock", product.Stock)
            };

            return await DbHelper.ExecuteNonQuery("UpdateProduct", parameters, _connectionStrings) > 0;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_productId", productId)
            };

            return await DbHelper.ExecuteNonQuery("DeleteProduct", parameters, _connectionStrings) > 0;
        }
    }
}
