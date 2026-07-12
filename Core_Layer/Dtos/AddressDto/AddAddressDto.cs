namespace Core_Layer.Dtos.AddressDto;

public class AddAddressDto
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string PostCode { get; set; }
    public string PhoneNumber { get; set; }
    public int ProvinceId { get; set; } 
    public int CityId { get; set; }
    public string FullAddress { get; set; }
}