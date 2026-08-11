namespace Data_Layer.Entities;

public class SnapShot
{
    public Guid Id { get; set; }
    public long ShippingCost { get; set; }
    public long TotalPrice { get; set; }
    public SnapShotState State { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<SnapShotItem> Items { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }  
    public string City { get; set; }
    public string Province { get; set; }
    public string FullAddress { get; set; }
    public string PostCode { get; set; }
    public string PhoneNumber { get; set; } 
    public string? Authority { get; set; }

}

public enum SnapShotState
{
    Pending = 0,
    Converted = 1,
    Expired = 2,
  

}