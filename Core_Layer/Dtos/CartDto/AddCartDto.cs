namespace Busines_Layer.Dtos.CartDto;

public class AddCartItemDto
{
    public int ProductId { get; set; }
    public string? UserId { get; set; }
    public int  Quantity { get; set; }  
}