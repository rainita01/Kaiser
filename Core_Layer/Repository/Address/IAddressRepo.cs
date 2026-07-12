using Core_Layer.Dtos.AddressDto;

namespace Core_Layer.Repository.Address;

public interface IAddressRepo
{
    public Task<ActionResult> AddAsync(AddAddressDto dto,string userId);
    public Task<ActionResult> UpdateAsync(UpdateAddressDto dto);
    public Task<ActionResult> DeleteAsync(int id);
    public Task<ActionResult> AddAddressToOrderAsync(AddAddressToOrderDto id);
    public Task<List<AddressDto>?> GetUserAddresses(string userId);
    public Task<ActionResult> AddProvinceAsync(string name);
    public Task<ActionResult> AddCityAsync(string name,int provinceId);
    public Task<ActionResult> DeleteCityAsync(int id);
    public Task<ActionResult> DeleteProvinceAsync(int id);
    public Task<List<ProviceDto>> GetProvinceAsync();
    public Task<List<CityDto>> GetCitiesAsync();
  

}