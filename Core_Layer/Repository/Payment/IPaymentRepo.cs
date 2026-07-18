using Core_Layer.Dtos.PaymentDto;

namespace Core_Layer.Repository.Payment;

public interface IPaymentRepo
{
    public Task<PaymentDto> GetPaymentAsync(Guid paymentId, string authority);
}