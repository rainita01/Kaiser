

namespace Core_Layer.Dtos.OrderDto;

using AddressDto;
using Data_Layer.Entities;

public class OrderDto
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public OrderState State { get; set; }

    public long ShippingCost { get; set; }

    public long TotalPrice { get; set; }

    public Guid PaymentId { get; set; }

    public Guid SnapShotId { get; set; }

    public int AddressId { get; set; }

    public string UserId { get; set; }

    public AddressDto Address { get; set; }

    public List<OrderItemDto> Items { get; set; }
}