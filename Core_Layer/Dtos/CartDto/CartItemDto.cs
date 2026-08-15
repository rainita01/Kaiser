using Busines_Layer.Dtos.Product;

namespace Busines_Layer.Dtos.CartDto;

public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string ImageName { get; set; }
    public long Price { get; set; }
    public int Quantity { get; set; }
    public int DiscountPercent { get; set; }
}
