
namespace Core_Layer.Dtos.OrderDto;

public class OrderItemDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public string ProductSlug { get; set; }

    public string ProductImage { get; set; }

    public long UnitPrice { get; set; }

    public decimal Discount { get; set; }

    public int Quantity { get; set; }

    public long TotalPrice =>
        (long)(UnitPrice * (1 - Discount / 100m) * Quantity);
}