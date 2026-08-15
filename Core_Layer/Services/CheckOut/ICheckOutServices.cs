using Busines_Layer.Dtos.PaymentDto;
using Busines_Layer.Dtos.SnapShotDto;
using Microsoft.AspNetCore.Mvc;

namespace Busines_Layer.Services.CheckOut;

public interface ICheckOutServices
{
    public Task<string> CheckOutAsync(string userId, int addressId);
    public Task<PaymentResultDto> HandleCallbackAsync(string authority, string status);
}