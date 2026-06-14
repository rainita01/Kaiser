namespace Data_Layer.Entities;

public class Address : BaseEntity
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public int PostCode { get; set; }
    public int PhoneNumber { get; set; }
    public string Province { get; set; }
    public string City { get; set; }
    public string FullAddress { get; set; }

    #region Relations

    public List<Order>? Orders { get; set; } 
    public string UserId { get; set; }
    public User User { get; set; }  

    #endregion
}