namespace Core_Layer.Repository.Payment;

public interface IPaymentRepo
{
    public Task<ActionResult> CreatePaymentAsync();
    public Task<bool> ValidationPaymentAsync();
}