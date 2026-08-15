
using Busines_Layer.Dtos.AddressDto;
using Busines_Layer.Dtos.Postex;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Busines_Layer.Services.Api.Postex;

public class PostexServices(HttpClient client,ILogger<PostexServices> logger) :IPostexServices
{

    public async Task<List<ProviceDto>?> GetProvincesAsync()
    {
        var req = await client.GetAsync( "locality/provinces");
        return await req.Content.ReadFromJsonAsync<List<ProviceDto>>();
    }

    public async Task<List<CityDto>?> GetAllCitiesAsync()
    {   
        var req = await client.GetAsync( "locality/cities/all");
        return await req.Content.ReadFromJsonAsync<List<CityDto>>();
    }

    public async Task<List<CityDto>?> GetCityByProvince(int provinceCode)
    {
        var req = await client.GetAsync( $"locality/cities/{provinceCode}");
        return await req.Content.ReadFromJsonAsync<List<CityDto>>();
    }

    public async Task<List<BoxDto>> GetBoxsInfo()
    {
        var req = await client.GetAsync("common/boxes");
        return await req.Content.ReadFromJsonAsync<List<BoxDto>>();

    }
    public async Task<double> CheckPrice(List<GetShippingQuotesQueryParcels> shippingDto)
    {
        try
        {
            var shipping = new GetShippingQuotesQueryDto
            {
                Parcels = shippingDto,
                Courier = new Courier()
                {
                    ServiceType = "EXPRESS",
                    CourierCode = "IR_POST"
                },
                CollectionType = "courier_drop_off",
                FromCityCode = 266,
                ValueAddedServices = new OptionalServices()
                {
                    RequestLabel = false,
                    RequestPackaging = false,
                    RequestSmsNotification = false,
                }
            };

            var req = await client.PostAsJsonAsync("shipping/quotes",shipping);
            var response = await req.Content.ReadFromJsonAsync<ShippingQuotesResponseDto>();
            if (response == null)
            {
                logger.LogError("connection failed with getting shipping costs");
                throw new HttpRequestException();
            }
            return response.ShippingPrice;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"exception while getting shipping costs");
            throw;
        }
    }
}