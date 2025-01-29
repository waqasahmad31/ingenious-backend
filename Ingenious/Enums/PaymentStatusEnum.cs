using System.Text.Json.Serialization;

namespace Ingenious.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentStatusEnum
    {
        Pending,
        Paid,
        Failed,
        Refunded
    }
}
