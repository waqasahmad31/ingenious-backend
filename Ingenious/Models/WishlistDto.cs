namespace Ingenious.Models
{
    public class WishlistDto
    {
        public int WishlistId { get; set; }
        public string AspNetUserId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetWishlistDto
    {
        public int WishlistId { get; set; }
        public string AspNetUserId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; }

        public GetProductDto Product { get; set; }
    }

    public class AddWishlistDto
    {
        public string AspNetUserId { get; set; }
        public int ProductId { get; set; }
    }
}
