using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core_Layer.Dtos.OrderDto;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core_Layer.Repository.Order;

public class OrderRepo(ILogger<OrderRepo> logger,Context context,IMapper mapper) : IOrderRepo
{

    public async Task<ActionResult> RemoveAsync(int orderId)
    {
        try
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null)
            {
                logger.LogWarning("order: {id} not found",orderId);
                return ActionResult.Failed("سفارش پیدا نشد");
            }
          
            context.Orders.Remove(order);
            await context.SaveChangesAsync();
            logger.LogInformation("order: {id} removed  successfully",orderId);
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while removing order : {id}",orderId);
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<List<OrderDto>> GetOrderListAsync(string userId)
    {
        try
        {
            return await context.Orders
                .AsNoTracking()
                .Where(e => e.UserId == userId)
                .ProjectTo<OrderDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while getting orders for user: {id}",userId);
            throw;
        }
    }

    public async Task<List<OrderDto>> GetAllOrderListAsync()
    {
        try
        {
            return await context.Orders
                .AsNoTracking()
                .OrderByDescending(e => e.CreatedAt)
                .ProjectTo<OrderDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while get all orders list");
            throw;
        }
    }

    public async Task<ActionResult> CreateOrderAsync(AddOrderDto dto, string userId)
    {
        try
        {
            var order = mapper.Map<Data_Layer.Entities.Order>(dto);

            order.UserId = userId;
            await context.Orders.AddAsync(order);
            logger.LogInformation("new order created for user: {userId}",userId);
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while adding order by user:{userId}",userId);
            return ActionResult.Failed(e.Message);
        }

    }
}