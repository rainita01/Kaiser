using Busines_Layer.Dtos.PaymentDto;

namespace Busines_Layer.Services.Api;

public interface IZarinPalServices
{
    public Task<ZarinPalResponseDto> RequestAsync(ZarinPalRequestDto dto);
    Task<ZarinPalVerifyResponseDto> VerifyAsync(ZarinPalVerifyDto dto);
}