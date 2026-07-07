using Core_Layer.Dtos.OrderDto;
using Core_Layer.Dtos.SnapShotDto;

namespace Core_Layer.Repository.Order;

public interface IOrderRepo
{
    public Task<ActionResult> RemoveAsync(int orderId);       
    public Task<List<OrderDto>> GetOrderListAsync(string userId);
    public Task<List<OrderDto>> GetAllOrderListAsync();
    public Task<ActionResult> CreateOrderAsync(AddOrderDto dto,string userId);

}