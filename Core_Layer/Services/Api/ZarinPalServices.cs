using Core_Layer.Dtos.PaymentDto;
using System.Net.Http.Json;
using System.Text.Json;

namespace Core_Layer.Services.Api;

public class ZarinPalServices(HttpClient httpClient) :IZarinPalServices
{
    public async Task<ZarinPalResponseDto> RequestAsync(ZarinPalRequestDto dto)
    {

        var response = await httpClient.PostAsJsonAsync("pg/v4/payment/request.json", dto);

        var body = await response.Content.ReadAsStringAsync();

        Console.WriteLine(body);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(body);
        }
        var result = await response.Content
            .ReadFromJsonAsync<ZarinPalResponseDto>();
        if (result == null)
        {
            throw new NullReferenceException();
        }
        return result;
    }

    public async Task<ZarinPalVerifyResponseDto> VerifyAsync(ZarinPalVerifyDto dto)
    {
        var response = await httpClient.PostAsJsonAsync("pg/v4/payment/verify.json", dto);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"ZarinPal verify failed: {body}");

        var result = JsonSerializer.Deserialize<ZarinPalVerifyResponseDto>(body)
                     ?? throw new Exception("پاسخ نامعتبر از درگاه پرداخت هنگام تایید");

        return result;
    }
}