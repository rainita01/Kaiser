namespace Core_Layer.Dtos.PaymentDto;

public class ZarinPalResponseDto
{
    public ZarinPalDataDto data { get; set; } = null!;
    public List<object>? errors { get; set; }
}

public class ZarinPalDataDto
{
    public int code { get; set; }
    public string message { get; set; } = null!;
    public string authority { get; set; } = null!;
    public string fee_type { get; set; } = null!;
    public long fee { get; set; }
}