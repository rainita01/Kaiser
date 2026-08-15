using Busines_Layer.Dtos.PaymentDto;

namespace Busines_Layer.Repository.Payment;

public interface IPaymentRepo
{
    public Task<PaymentDto> GetPaymentAsync(Guid paymentId, string authority);
}