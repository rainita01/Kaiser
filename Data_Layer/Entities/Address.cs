namespace Data_Layer.Entities;

public class Address : BaseEntity
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string PostCode { get; set; }
    public string PhoneNumber { get; set; }

    public string FullAddress { get; set; }

    #region Relations

    public int ProvinceId { get; set; }
    public int CityId { get; set; }
    public Province Province { get; set; }
    public City City { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }  

    #endregion
}

public class City
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Address> Addresses { get; set; }
    public int ProvinceId { get; set; }
    public Province Province { get; set; }  
}

public class Province
{
    public int Id { get; set; }
    public string  Name { get; set; }   

    public List<City> Cities { get; set; }
    public List<Address> Addresses { get; set; }   
}