namespace Ingenious.Models
{
    public class CreateProductDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Stock { get; set; }
    }

    public class ProductCreateCombinedDto
    {
        public CreateProductDto Product { get; set; }
        public List<CreateProductImageDto> Images { get; set; }
        public List<CreateProductVariationDto> Variations { get; set; }
    }

    public class GetProductDto
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public CategoryDto? Category { get; set; } = new CategoryDto();
        public List<ProductImageDto>? ProductImages { get; set; } = new List<ProductImageDto>();
        public List<ProductVariationDto>? ProductVariations { get; set; } = new List<ProductVariationDto>();
    }
}
