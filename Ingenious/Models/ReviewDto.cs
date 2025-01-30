namespace Ingenious.Models
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public string AspNetUserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetReviewDto
    {
        public int ReviewId { get; set; }
        public string AspNetUserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public GetProductDto Product { get; set; }
    }

    public class AddReviewDto
    {
        public string AspNetUserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

    public class UpdateReviewDto
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

}
