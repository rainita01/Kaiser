using Core_Layer.Dtos.PaymentDto;
using Core_Layer.Dtos.ZarinPalDto;

namespace Core_Layer.Services.Api;

public interface IZarinPalServices
{
    public Task<ZarinPalResponseDto> RequestAsync(ZarinPalRequestDto dto);
    Task<ZarinPalVerifyResponseDto> VerifyAsync(ZarinPalVerifyDto dto);
}