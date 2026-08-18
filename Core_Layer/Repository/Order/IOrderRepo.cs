using Busines_Layer.Dtos.OrderDto;
using Busines_Layer.Dtos.SnapShotDto;
using Data_Layer.Entities;

namespace Busines_Layer.Repository.Order;

public interface IOrderRepo
{
    public Task<ActionResult> RemoveAsync(int orderId);       
    public Task<List<OrderDto>> GetOrderListAsync(string userId);
    public Task<List<OrderDto>> GetAllOrderListAsync();
    public Task<ActionResult> CreateOrderAsync(AddOrderDto dto,string userId);
    public Task<OrderDto> ChangeStateAsync(OrderState state, int orderId);

}