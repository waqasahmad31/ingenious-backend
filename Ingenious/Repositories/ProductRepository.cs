using DAS.DataAccess.Helpers;
using Ingenious.Models;
using Microsoft.Extensions.Caching.Memory;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
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
        Task<bool> UpdateProductWithDetailsAsync(int productId, ProductCreateCombinedDto productDto);
        Task<int> AddProductAsync(CreateProductDto product);
        Task<bool> UpdateProductAsync(int productId, CreateProductDto product);
        Task<bool> DeleteProductAsync(int productId);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly IMemoryCache _cache;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IProductVariationRepository _productVariationRepository;
        private readonly ConnectionStrings _connectionStrings;

        public ProductRepository(
            ConnectionStrings connectionStrings,
            IProductImageRepository productImageRepository,
            IProductVariationRepository productVariationRepository,
            IMemoryCache cache
        )
        {
            _productImageRepository = productImageRepository;
            _productVariationRepository = productVariationRepository;
            _cache = cache;
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
            // Dynamically generate a cache key based on filters
            var cacheKey = GenerateCacheKey(productId, categoryId, name, minPrice, maxPrice, minStock, maxStock, isActive);

            // Check if data exists in cache
            if (_cache.TryGetValue(cacheKey, out string cachedData))
            {
                return JsonConvert.DeserializeObject<IEnumerable<GetProductDto>>(cachedData);
            }

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

            // Cache the result for 5 minutes
            _cache.Set(cacheKey, JsonConvert.SerializeObject(products), new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache for 5 minutes
            });

            return products;
        }

        private string GenerateCacheKey(
            int? productId,
            int? categoryId,
            string? name,
            decimal? minPrice,
            decimal? maxPrice,
            int? minStock,
            int? maxStock,
            bool? isActive)
        {
            var key = $"productList_";

            if (productId.HasValue) key += $"productId_{productId}_";
            if (categoryId.HasValue) key += $"categoryId_{categoryId}_";
            if (!string.IsNullOrEmpty(name)) key += $"name_{name}_";
            if (minPrice.HasValue) key += $"minPrice_{minPrice}_";
            if (maxPrice.HasValue) key += $"maxPrice_{maxPrice}_";
            if (minStock.HasValue) key += $"minStock_{minStock}_";
            if (maxStock.HasValue) key += $"maxStock_{maxStock}_";
            if (isActive.HasValue) key += $"isActive_{isActive}_";

            return key.TrimEnd('_'); // Remove trailing underscore
        }

        public async Task<GetProductDto> GetProductByIdAsync(int productId)
        {
            var products = await GetAllProductsAsync(productId);

            var product = products.FirstOrDefault();

            return product;
        }

        // public async Task<bool> AddProductWithDetailsAsync(ProductCreateCombinedDto productDto)
        // {
        //     using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        //     try
        //     {
        //         // Add product
        //         var productId = await AddProductAsync(productDto.Product);

        //         // Add images
        //         foreach (var image in productDto.Images)
        //         {
        //             image.ProductId = productId;
        //             await _productImageRepository.AddImageAsync(image);
        //         }

        //         // Add variations
        //         foreach (var variation in productDto.Variations)
        //         {
        //             variation.ProductId = productId;
        //             await _productVariationRepository.AddVariationAsync(variation);
        //         }

        //         transaction.Complete();
        //         return true;
        //     }
        //     catch (Exception ex)
        //     {
        //         // Log error here
        //         return false;
        //     }
        // }

        public async Task<bool> AddProductWithDetailsAsync(ProductCreateCombinedDto productDto)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                // Add product to the database
                var productId = await AddProductAsync(productDto.Product);

                // Add images and variations (optional, based on your implementation)
                var imageInsertTasks = productDto.Images.Select(image =>
                {
                    image.ProductId = productId;
                    return _productImageRepository.AddImageAsync(image);
                }).ToList();

                var variationInsertTasks = productDto.Variations.Select(variation =>
                {
                    variation.ProductId = productId;
                    return _productVariationRepository.AddVariationAsync(variation);
                }).ToList();

                // Wait for all insert operations to complete in parallel
                await Task.WhenAll(imageInsertTasks.Concat(variationInsertTasks));

                // Invalidate the product list cache when a new product is added
                _cache.Remove("productList_*"); // Clear cache for product list

                transaction.Complete();
                return true;
            }
            catch (Exception ex)
            {
                // Log error here
                // Log.Logger.Error($"Error adding product with details: {ex.Message}");
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

            var result = await DbHelper.ExecuteNonQuery("CreateProduct", parameters, _connectionStrings);
            _cache.Remove("productList_*");
            return result;
        }

        public async Task<bool> UpdateProductWithDetailsAsync(int productId, ProductCreateCombinedDto productDto)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                // Update product details
                var isProductUpdated = await UpdateProductAsync(productId, productDto.Product);

                // Check if product update was successful
                if (!isProductUpdated)
                    return false;

                // Update images
                // Delete existing images if necessary (or mark them inactive, depending on your logic)
                var existingImages = await _productImageRepository.GetImagesByProductIdAsync(productId);
                foreach (var existingImage in existingImages)
                {
                    // You can decide to delete existing images or update them.
                    // For now, deleting them for simplicity.
                    await _productImageRepository.DeleteImageAsync(existingImage.ImageId);
                }

                // Add new images
                var imageInsertTasks = productDto.Images.Select(image =>
                {
                    image.ProductId = productId;
                    return _productImageRepository.AddImageAsync(image);
                }).ToList();

                // Add variations
                var variationInsertTasks = productDto.Variations.Select(variation =>
                {
                    variation.ProductId = productId;
                    return _productVariationRepository.AddVariationAsync(variation);
                }).ToList();

                // Wait for all insert operations to complete in parallel
                await Task.WhenAll(imageInsertTasks.Concat(variationInsertTasks));

                // Invalidate the product list cache when a product is updated
                _cache.Remove("productList_*"); // Clear cache for product list

                transaction.Complete();
                return true;
            }
            catch (Exception ex)
            {
                // Log error here
                // Log.Logger.Error($"Error updating product with details: {ex.Message}");
                return false;
            }
        }


        // public async Task<bool> UpdateProductAsync(int productId, CreateProductDto product)
        // {
        //     var parameters = new[]
        //     {
        //         new MySqlParameter("@p_productId", productId),
        //         new MySqlParameter("@p_categoryId", product.CategoryId),
        //         new MySqlParameter("@p_name", product.Name),
        //         new MySqlParameter("@p_slug", product.Slug),
        //         new MySqlParameter("@p_description", product.Description ?? (object)DBNull.Value),
        //         new MySqlParameter("@p_price", product.Price),
        //         new MySqlParameter("@p_discount", product.Discount),
        //         new MySqlParameter("@p_stock", product.Stock)
        //     };

        //     return await DbHelper.ExecuteNonQuery("UpdateProduct", parameters, _connectionStrings) > 0;
        // }


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

            var isUpdated = await DbHelper.ExecuteNonQuery("UpdateProduct", parameters, _connectionStrings) > 0;

            if (isUpdated)
            {
                // Invalidate cache for this product
                var cacheKey = "productList_" + productId;
                _cache.Remove(cacheKey); // Remove the specific product cache
                _cache.Remove("productList_*"); // Optionally clear the general product list cache as well

                // Optionally update the cache with the new data
                var updatedProduct = await GetProductByIdAsync(productId); // Get the updated product from DB
                _cache.Set(cacheKey, JsonConvert.SerializeObject(updatedProduct), new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // Cache it again for 5 minutes
                });
            }

            return isUpdated;
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
