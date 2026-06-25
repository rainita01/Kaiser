namespace Core_Layer.Dtos.AddressDto;

public class AddAddressDto
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public int PostCode { get; set; }
    public int PhoneNumber { get; set; }
    public string Province { get; set; }
    public string City { get; set; }
    public string FullAddress { get; set; }
}