using System.ComponentModel.DataAnnotations;

namespace Data_Layer.Entities;

public class Order 
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; }
    public long ShippingCost { get; set; }
    public long TotalPrice { get; set; }
    public OrderState State { get; set; } // Paid, Processing, Shipped, Delivered, Cancelled
    public DateTime CreatedAt { get; set; }
    #region Relations

    public int AddressId { get; set; }
    public Address Address { get; set; } = new Address();
    public Guid SnapShotId { get; set; }
    public SnapShot SnapShot { get; set; }  
    public Guid PaymentId { get; set; }
    public Payment Payment { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }  

    #endregion
}

public enum OrderState { Paid, Processing, Shipped, Delivered, Cancelled }