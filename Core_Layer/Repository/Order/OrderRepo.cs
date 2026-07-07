using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core_Layer.Dtos.OrderDto;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;
namespace Core_Layer.Repository.Order;

public class OrderRepo(Context context,IMapper mapper) : IOrderRepo
{

    public async Task<ActionResult> RemoveAsync(int orderId)
    {
        try
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null)
                return ActionResult.Failed("سفارش پیدا نشد");
          
            context.Orders.Remove(order);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<List<OrderDto>> GetOrderListAsync(string userId)
    {
        return await context.Orders
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .ProjectTo<OrderDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<List<OrderDto>> GetAllOrderListAsync()
    {
        return await context.Orders
            .AsNoTracking()
            .OrderByDescending(e=>e.CreatedAt)
            .ProjectTo<OrderDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ActionResult> CreateOrderAsync(AddOrderDto dto, string userId)
    {
        try
        {
            var order = mapper.Map<Data_Layer.Entities.Order>(dto);

            order.UserId = userId;
            await context.Orders.AddAsync(order);
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }

    }
}