namespace Busines_Layer.Dtos.AddressDto;

public class AddAddressDto
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string PostCode { get; set; }
    public string PhoneNumber { get; set; }
    public int city_code { get; set; }
    public string city_name { get; set; }
    public int province_code { get; set; }
    public string province_name { get; set; }
    public string FullAddress { get; set; }
}