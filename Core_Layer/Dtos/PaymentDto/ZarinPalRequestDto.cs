using System.Text.Json.Serialization;

namespace Busines_Layer.Dtos.PaymentDto;

public class ZarinPalRequestDto
{
    [JsonPropertyName("merchant_id")]
    public string MerchantId { get; set; } = null!;

    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; } = "IRR";

    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;

    [JsonPropertyName("callback_url")]
    public string CallbackUrl { get; set; } = null!;

    [JsonPropertyName("referrer_id")]
    public string? ReferrerId { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string,string>? Metadata { get; set; } = new Dictionary<string,string>();
}