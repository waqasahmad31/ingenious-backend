namespace Ingenious.Models
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? Slug { get; set; }
        public int? ParentCategoryId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class UpsertCategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int? ParentCategoryId { get; set; } = null;
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
