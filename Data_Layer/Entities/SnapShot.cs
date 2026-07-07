namespace Data_Layer.Entities;

public class SnapShot
{
    public Guid Id { get; set; }
    public long ShippingCost { get; set; }
    public long TotalPrice { get; set; }
    public SnapShotState State { get; set; } // Pending, Converted, Expired
    public DateTime CreatedAt { get; set; }
    public List<SnapShotItem> Items { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }  
    public int AddressId { get; set; }
    public Address Address { get; set; }
    public Payment? Payment { get; set; }

}

public enum SnapShotState
{
    Pending = 0,
    Converted = 1,
    Expired = 2,
  

}