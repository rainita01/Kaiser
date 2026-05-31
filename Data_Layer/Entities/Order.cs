using System.ComponentModel.DataAnnotations;

namespace Data_Layer.Entities;

public class Order 
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime OrderTime { get; set; }
    [Required]
    public OrderStatus Status { get; set; }
    [Required]
    public long TotalAmount { get; set; }

    #region Relations

    public int PaymentId { get; set; }
    public Payment Payment { get; set; }    
    public List<OrderItem> OrderItems { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }  

    #endregion
}

public enum OrderStatus
{
    Pending,
    Shipping,
    Delivered,
    Canceled    
}