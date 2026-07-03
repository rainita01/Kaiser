namespace Data_Layer.Entities;

public class SnapShot
{
    public Guid Id { get; set; }
    
    public long ShippingCost { get; set; }  // هزینه ارسال
    public long TotalPrice { get; set; }  
    public SnapShotState State { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime ExpiresAt => DateTime.Now + TimeSpan.FromMinutes(15);

    public ICollection<SnapShotItem> Items { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }  
    public int AddressId { get; set; }
    public Address Address { get; set; }

}

public enum SnapShotState
{
    Pending = 0,
    Paid = 1,
    Expires = 2

}