namespace Core_Layer.Dtos.OrderDto;

public class OrderItemDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public long UnitPrice { get; set; }
    public int ProductId { get; set; }
    public int OrderId { get; set; }
}