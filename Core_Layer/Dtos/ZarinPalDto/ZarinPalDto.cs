namespace Core_Layer.Dtos.ZarinPalDto;


// DTOهای پاسخ زرین‌پال
public class ZarinPalRequestResponse
{
    public ZarinPalRequestData? Data { get; set; }
    public ZarinPalError? Errors { get; set; }
}
public class ZarinPalRequestData
{
    public int Code { get; set; }
    public string Authority { get; set; } = default!;
}
public class ZarinPalVerifyResponse
{
    public ZarinPalVerifyData? Data { get; set; }
    public ZarinPalError? Errors { get; set; }
}
public class ZarinPalVerifyData
{
    public int Code { get; set; }
    public long RefId { get; set; }
}
public class ZarinPalError
{
    public string? Message { get; set; }
}