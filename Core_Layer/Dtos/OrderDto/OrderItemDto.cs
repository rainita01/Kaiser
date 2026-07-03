
namespace Core_Layer.Dtos.OrderDto;

public class OrderItemDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public long UnitPrice { get; set; }
    public string ProductName { get; set; }
    public int OrderId { get; set; }
    public long TotalPrice => UnitPrice * Quantity;
    
}