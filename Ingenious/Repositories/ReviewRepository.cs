using DAS.DataAccess.Helpers;
using Ingenious.Models;
using MySql.Data.MySqlClient;

namespace Ingenious.Repositories
{
    public interface IReviewRepository
    {
        Task<int> AddReviewAsync(AddReviewDto dto);
        Task<int> UpdateReviewAsync(UpdateReviewDto dto);
        Task<int> DeleteReviewAsync(int reviewId);
        Task<GetReviewDto> GetReviewByIdAsync(int reviewId);
        Task<List<GetReviewDto>> GetReviewsByProductIdAsync(int productId);
    }

    public class ReviewRepository : IReviewRepository
    {
        private readonly IProductRepository _productRepository;
        private readonly ConnectionStrings _connectionStrings;

        public ReviewRepository(IProductRepository productRepository, ConnectionStrings connectionStrings)
        {
            _productRepository = productRepository;
            _connectionStrings = connectionStrings;
        }

        public async Task<int> AddReviewAsync(AddReviewDto dto)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_AspNetUserId", dto.AspNetUserId),
                new MySqlParameter("@p_ProductId", dto.ProductId),
                new MySqlParameter("@p_Rating", dto.Rating),
                new MySqlParameter("@p_Comment", dto.Comment)
            };

            return await DbHelper.ExecuteWithOutput("Reviews_Add", parameters, "ReviewId", _connectionStrings);
        }

        public async Task<int> UpdateReviewAsync(UpdateReviewDto dto)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_ReviewId", dto.ReviewId),
                new MySqlParameter("@p_Rating", dto.Rating),
                new MySqlParameter("@p_Comment", dto.Comment)
            };

            return await DbHelper.ExecuteQuery("Reviews_Update", parameters, _connectionStrings);
        }

        public async Task<int> DeleteReviewAsync(int reviewId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_ReviewId", reviewId)
            };

            return await DbHelper.ExecuteQuery("Reviews_Delete", parameters, _connectionStrings);
        }

        public async Task<GetReviewDto> GetReviewByIdAsync(int reviewId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_ReviewId", reviewId)
            };

            return await DbHelper.Get<GetReviewDto>("Reviews_GetById", parameters, _connectionStrings);
        }

        public async Task<List<GetReviewDto>> GetReviewsByProductIdAsync(int productId)
        {
            var parameters = new[]
            {
                new MySqlParameter("@p_ProductId", productId)
            };

            var reviews = await DbHelper.GetList<ReviewDto>("Reviews_GetByProductId", parameters, _connectionStrings);

            List<GetReviewDto> getReviewsDto = new List<GetReviewDto>();

            foreach (var review in reviews)
            {
                GetReviewDto getReviewDto = new GetReviewDto()
                {
                    ReviewId = review.ReviewId,
                    AspNetUserId = review.AspNetUserId,
                    ProductId = review.ProductId,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt,
                    //Product = await _productRepository.GetProductByIdAsync(review.ProductId)
                    Product = null
                };

                getReviewsDto.Add(getReviewDto);
            }

            return getReviewsDto;
        }
    }
}
