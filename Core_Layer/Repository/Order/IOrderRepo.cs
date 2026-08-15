using Busines_Layer.Dtos.OrderDto;
using Busines_Layer.Dtos.SnapShotDto;

namespace Busines_Layer.Repository.Order;

public interface IOrderRepo
{
    public Task<ActionResult> RemoveAsync(int orderId);       
    public Task<List<OrderDto>> GetOrderListAsync(string userId);
    public Task<List<OrderDto>> GetAllOrderListAsync();
    public Task<ActionResult> CreateOrderAsync(AddOrderDto dto,string userId);

}