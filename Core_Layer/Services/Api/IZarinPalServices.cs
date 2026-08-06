using Core_Layer.Dtos.PaymentDto;

namespace Core_Layer.Services.Api;

public interface IZarinPalServices
{
    public Task<ZarinPalResponseDto> RequestAsync(ZarinPalRequestDto dto);
    Task<ZarinPalVerifyResponseDto> VerifyAsync(ZarinPalVerifyDto dto);
}