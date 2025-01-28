namespace Ingenious.Models
{
    public class ProductVariationDto
    {
        public int VariationId { get; set; }
        public int ProductId { get; set; }
        public string VariationName { get; set; }
        public decimal? Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateProductVariationDto
    {
        public int ProductId { get; set; }
        public string VariationName { get; set; }
        public decimal? Price { get; set; }
        public int Stock { get; set; }
    }
}
