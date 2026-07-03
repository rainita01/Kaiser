using Core_Layer.Dtos.OrderDto;
using Core_Layer.Dtos.SnapShotDto;

namespace Core_Layer.Repository.Order;

public interface IOrderRepo
{
    public Task<SnapShotDto> CheckOutAsync(string userId, int addressId);
    public Task<ActionResult> ChangeOrderStateAsync();
    public Task<List<OrderDto>> GetOrderListAsync(string userId);
    public Task<AddOrderDto> CreateOrderAsync(AddOrderDto dto,string userId);

}