using Core_Layer.Dtos.OrderDto;
using Core_Layer.Dtos.SnapShotDto;

namespace Core_Layer.Repository.Order;

public interface IOrderRepo
{
    public Task<SnapShotDto> AddAsync(string userId, int addressId);
    public Task<ActionResult> RemoveAsync();       
    public Task<List<OrderDto>> GetOrderListAsync(string userId);
    public Task<ActionResult> CreateOrderAsync(AddOrderDto dto,string userId);

}