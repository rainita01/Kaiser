using System.Text.Json.Serialization;

namespace Core_Layer.Dtos.PaymentDto;

public class ZarinPalVerifyDto
{
    [JsonPropertyName("merchant_id")]
    public string MerchantId { get; set; } = null!;

    [JsonPropertyName("amount")]
    public long Amount { get; set; }
    [JsonPropertyName("authority")]
    public string Authority { get; set; }
}