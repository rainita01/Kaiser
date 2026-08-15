using System.Text.Json.Serialization;

namespace Busines_Layer.Dtos.PaymentDto;

public class ZarinPalVerifyResponseDto
{
    [JsonPropertyName("data")]
    public ZarinPalVerifyDataDto Data { get; set; } = null!;

    [JsonPropertyName("errors")]
    public List<object> Errors { get; set; } = [];
}


public class ZarinPalVerifyDataDto
{
    [JsonPropertyName("wages")]
    public object? Wages { get; set; }

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;

    [JsonPropertyName("card_hash")]
    public string? CardHash { get; set; }

    [JsonPropertyName("card_pan")]
    public string? CardPan { get; set; }

    [JsonPropertyName("ref_id")]
    public long RefId { get; set; }

    [JsonPropertyName("fee_type")]
    public string? FeeType { get; set; }

    [JsonPropertyName("fee")]
    public long Fee { get; set; }

    [JsonPropertyName("shaparak_fee")]
    public long ShaparakFee { get; set; }

    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}