using Core_Layer.Dtos.PaymentDto;
using Core_Layer.Dtos.SnapShotDto;
using Microsoft.AspNetCore.Mvc;

namespace Core_Layer.Services.CheckOut;

public interface ICheckOutServices
{
    public Task<Guid> CheckOutAsync(string userId, int addressId);
    public Task<string> StartPaymentAsync(string userId, Guid snapshotId);
    public Task<PaymentResultDto> HandleCallbackAsync(Guid paymentId, string authority, string status);
}