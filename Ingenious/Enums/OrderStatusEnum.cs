using System.Text.Json.Serialization;

namespace Ingenious.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatusEnum
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}
