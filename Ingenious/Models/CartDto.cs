namespace Ingenious.Models
{
    public class GetCartDto
    {
        public int CartId { get; set; }
        public string AspNetUserId { get; set; }
        public int ProductId { get; set; }
        public int? VariationId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string? ProductName { get; set; }
        public string? VariationName { get; set; }
        public string? ProductImageUrl { get; set; }
    }

    public class CreateCartDto
    {
        public string AspNetUserId { get; set; }
        public int ProductId { get; set; }
        public int? VariationId { get; set; } = null;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateCartDto
    {
        public int CartId { get; set; }
        public string AspNetUserId { get; set; }
        public int ProductId { get; set; }
        public int? VariationId { get; set; } = null;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
