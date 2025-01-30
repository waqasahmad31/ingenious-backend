using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    //public enum OrderStatusEnum
    //{
    //    Pending = "Pending",
    //    Processing = "Processing",
    //    Shipped = "Shipped",
    //    Delivered = "Delivered",
    //    Cancelled = "Cancelled"
    //}

    //function getOrderStatusLabel(status: number): string {
    //  return Object.values(OrderStatusEnum)[status] || "Unknown";
    //}
}
