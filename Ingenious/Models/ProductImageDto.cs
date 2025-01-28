namespace Ingenious.Models
{
    public class ProductImageDto
    {
        public int ImageId { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateProductImageDto
    {
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsDefault { get; set; }
    }
}
