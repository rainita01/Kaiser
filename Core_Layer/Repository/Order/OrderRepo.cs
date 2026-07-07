
using System.Data.SqlTypes;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core_Layer.Dtos.OrderDto;
using Core_Layer.Dtos.SnapShotDto;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core_Layer.Repository.Order;

public class OrderRepo(Context context,IMapper mapper) : IOrderRepo
{


    public Task<ActionResult> PaymentResponseAsync()
    {
        throw new NotImplementedException();
    }

    public Task<SnapShotDto> AddAsync(string userId, int addressId)
    {
        throw new NotImplementedException();
    }

    public Task<ActionResult> RemoveAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<List<OrderDto>> GetOrderListAsync(string userId)
    {
        return await context.Orders
            .AsNoTracking()
            .Where(e => e.UserId == userId)
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