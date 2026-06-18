using Core_Layer.Dtos.CartDto;

namespace Core_Layer.Repository.Cart;

public interface ICartRepo
{
    public Task<ActionResult> AddItemAsync(AddCartItemDto itemDto);
    public Task<ActionResult> RemoveItemAsync(int cardItemId);
    public Task<ActionResult> IncreaseQuantityAsync(int cardItemId); 
    public Task<ActionResult> DecreaseQuantityAsync(int cardItemId);
    public Task<List<CartItemDto>> GetUserCartItemsAsync(string userId);
}