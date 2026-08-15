namespace Busines_Layer.Dtos.OrderDto;

public class AddOrderItemDto
{
    public long UnitPrice { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int OrderId { get; set; }
}