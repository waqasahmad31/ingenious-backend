using DAS.DataAccess.Helpers;
using Ingenious.Models;
using MySql.Data.MySqlClient;

namespace Ingenious.Repositories
{
    public interface IWishlistRepository
    {
        Task<int> AddToWishlistAsync(AddWishlistDto dto);
        Task<int> RemoveFromWishlistAsync(int wishlistId);
        Task<List<GetWishlistDto>> GetWishlistByUserIdAsync(string aspNetUserId);
        Task<GetWishlistDto> GetWishlistByIdAsync(int wishlistId);
    }

    public class WishlistRepository : IWishlistRepository
    {
        private readonly IProductRepository _productRepository;
        private readonly ConnectionStrings _connectionStrings;

        public WishlistRepository(IProductRepository productRepository, ConnectionStrings connectionStrings)
        {
            _productRepository = productRepository;
            _connectionStrings = connectionStrings;
        }

        public async Task<int> AddToWishlistAsync(AddWishlistDto dto)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_AspNetUserId", dto.AspNetUserId),
                new MySqlParameter("@p_ProductId", dto.ProductId)
            };

            return await DbHelper.ExecuteNonQuery("Wishlist_Add", parameters, _connectionStrings);
        }

        public async Task<int> RemoveFromWishlistAsync(int wishlistId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_WishlistId", wishlistId)
            };

            return await DbHelper.ExecuteQuery("Wishlist_Remove", parameters, _connectionStrings);
        }

        public async Task<List<GetWishlistDto>> GetWishlistByUserIdAsync(string aspNetUserId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_AspNetUserId", aspNetUserId)
            };

            var wishlists = await DbHelper.GetList<WishlistDto>("Wishlist_GetByUserId", parameters, _connectionStrings);

            List<GetWishlistDto> getWishlistsDto = new List<GetWishlistDto>();

            foreach (var wishlist in wishlists)
            {
                GetWishlistDto wishlistDto = new GetWishlistDto()
                {
                    WishlistId = wishlist.WishlistId,
                    AspNetUserId = wishlist.AspNetUserId,
                    ProductId = wishlist.ProductId,
                    CreatedAt = wishlist.CreatedAt,
                    Product = await _productRepository.GetProductByIdAsync(wishlist.ProductId)
                };

                getWishlistsDto.Add(wishlistDto);
            }

            return getWishlistsDto;
        }

        public async Task<GetWishlistDto> GetWishlistByIdAsync(int wishlistId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_WishlistId", wishlistId)
            };

            var wishlist = await DbHelper.Get<WishlistDto>("Wishlist_GetById", parameters, _connectionStrings);

            GetWishlistDto wishlistDto = new GetWishlistDto()
            {
                WishlistId = wishlist.WishlistId,
                AspNetUserId = wishlist.AspNetUserId,
                ProductId = wishlist.ProductId,
                CreatedAt = wishlist.CreatedAt,
                Product = await _productRepository.GetProductByIdAsync(wishlist.ProductId)
            };

            return wishlistDto;
        }
    }
}
