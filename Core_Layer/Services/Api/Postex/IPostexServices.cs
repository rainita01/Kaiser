using Busines_Layer.Dtos.AddressDto;
using Busines_Layer.Dtos.Postex;

namespace Busines_Layer.Services.Api.Postex;

public interface IPostexServices
{
    Task<List<ProviceDto>?> GetProvincesAsync();
    Task<List<CityDto>?> GetAllCitiesAsync();
    Task<List<CityDto>?> GetCityByProvince(int provinceCode);
    Task<double> CheckPrice(List<GetShippingQuotesQueryParcels> shippingDto);
     Task<List<BoxDto>> GetBoxsInfo();
}