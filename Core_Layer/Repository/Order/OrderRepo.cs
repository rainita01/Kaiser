
using System.Data.SqlTypes;
using AutoMapper;
using Core_Layer.Dtos.OrderDto;
using Core_Layer.Dtos.SnapShotDto;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core_Layer.Repository.Order;

public class OrderRepo(Context context,IMapper mapper) : IOrderRepo
{
    public async Task<SnapShotDto> CheckOutAsync(string userId,int addressId)
    {
        var cart = await context.Carts
            .AsNoTracking()
            .Include(e=>e.CartItems)
            .ThenInclude(e=>e.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null || cart.CartItems == null)
            throw new FileNotFoundException("کارتی پیدا نشد");
        var address = await context.Addresses
            .FindAsync(addressId);
        if (address == null)
            throw new SqlNullValueException("ادرس پیدا نشد");

        var snapshot = new SnapShotDto()
        {
            AddressId = addressId,
            Items = mapper.Map<List<SnapShotItemDto>>(cart.CartItems),
            ShippingCost = 1000000,
            State = SnapShotState.Pending,
            UserId = userId
        };

        long totalPrice = 0;
        foreach (var item in snapshot.Items)
        {
            totalPrice += item.UnitPrice;
        }
        snapshot.TotalPrice = totalPrice;
        return snapshot;

    }

    public Task<ActionResult> ChangeOrderStateAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<OrderDto>> GetOrderListAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<AddOrderDto> CreateOrderAsync(AddOrderDto dto, string userId)
    {
        throw new NotImplementedException();
    }
}