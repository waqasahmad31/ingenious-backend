using Ingenious.Enums;
using System.Text.Json.Serialization;

namespace Ingenious.Models
{
    public class GetOrderDto
    {
        public int OrderId { get; set; }
        public string AspNetUserId { get; set; }
        public decimal TotalAmount { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatusEnum OrderStatus { get; set; } //ENUM('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled')
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentStatusEnum PaymentStatus { get; set; } //ENUM('Pending', 'Paid', 'Failed', 'Refunded')
        public int ShippingAddressId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<GetOrderItemsDto> OrderItems { get; set; }
    }

    public class GetOrderItemsDto
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int VariationId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class AddOrderDto
    {
        public string AspNetUserId { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatusEnum OrderStatus { get; set; } //ENUM('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled')
        public PaymentStatusEnum PaymentStatus { get; set; } //ENUM('Pending', 'Paid', 'Failed', 'Refunded')
        public int ShippingAddressId { get; set; }

        public List<AddOrderItemDto> OrderItems { get; set; }
    }

    public class AddOrderItemDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int VariationId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
