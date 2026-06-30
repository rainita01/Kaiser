using Data_Layer.Entities;

namespace Core_Layer.Dtos.OrderDto;

public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderTime { get; set; }
    public OrderStatus Status { get; set; }   
    public long TotalAmount { get; set; }
    public int AddressId { get; set; }
    public int  PaymentId { get; set; }
    public string UserId { get; set; }
    public List<OrderItemDto> List { get; set; }
}