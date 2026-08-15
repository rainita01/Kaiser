using AutoMapper;
using AutoMapper.QueryableExtensions;
using Busines_Layer.Dtos.PaymentDto;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Busines_Layer.Repository.Payment;

public class PaymentRepo(IMapper mapper , Context context,ILogger<PaymentRepo> logger) : IPaymentRepo
{
    public async Task<PaymentDto> GetPaymentAsync(Guid paymentId, string authority)
    {
        var payment = await context.Payments
            .Include(p => p.SnapShot)
            .ThenInclude(s => s.Items)
            .ProjectTo<PaymentDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(p => p.Id == paymentId && p.Authority == authority);

        if (payment == null)
        {
            logger.LogError("payment not found: {id} with authorize {authorize}",paymentId,authority);
            throw new Exception("پرداخت یافت نشد");
        }

        return payment;

    }
}