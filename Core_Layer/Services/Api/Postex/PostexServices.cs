using System.Buffers;
using System.IO.Pipelines;
using Busines_Layer.Dtos.AddressDto;
using Busines_Layer.Dtos.Postex;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;


namespace Busines_Layer.Services.Api.Postex;

public class PostexServices(HttpClient client) :IPostexServices
{
    private string baseUrl = "https://api.postex.ir/api/v1/";
    private string token = "postex_live_b872ae0b34YLPEbtAt2m7GQtqbE95REk8FMKdtSQ";

    public async Task<List<ProviceDto>?> GetProvincesAsync()
    {
        var req = await client.GetAsync(baseUrl + "locality/provinces");
        return await req.Content.ReadFromJsonAsync<List<ProviceDto>>();
    }

    public async Task<List<CityDto>?> GetAllCitiesAsync()
    {   
        var req = await client.GetAsync(baseUrl + "locality/cities/all");
        return await req.Content.ReadFromJsonAsync<List<CityDto>>();
    }

    public async Task<List<CityDto>?> GetCityByProvince(int provinceCode)
    {
        var req = await client.GetAsync(baseUrl + $"locality/cities/{provinceCode}");
        return await req.Content.ReadFromJsonAsync<List<CityDto>>();
    }

    public async Task<double?> CheckPrice(GetShippingQuotesQueryDto shippingDto)
    {
        client.DefaultRequestHeaders.Add("x-api-key", token);
        try
        {
            var req = await client.PostAsJsonAsync(baseUrl + "shipping/quotes",shippingDto);
            var response = await req.Content.ReadFromJsonAsync<ShippingQuotesResponse>();

            var totalPrice = response?
                .ShippingPrices
                .FirstOrDefault()?
                .ServicePrice
                .FirstOrDefault()?
                .TotalPrice;
                return totalPrice;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }
}