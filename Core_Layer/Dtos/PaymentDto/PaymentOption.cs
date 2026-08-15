namespace Busines_Layer.Dtos.PaymentDto;

public class PaymentOption
{
    public string BaseUrl { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
}