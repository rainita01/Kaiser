namespace Data_Layer.Entities;

public class Address : BaseEntity
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string PostCode { get; set; }
    public string PhoneNumber { get; set; }

    public string FullAddress { get; set; }

    #region Relations

    public int province_code { get; set; }
    public string province_name { get; set; }
    public int city_code { get; set; }
    public string city_name { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }  

    #endregion
}