namespace Core_Layer.Repository.Order;

public interface IOrderRepo
{
    public Task<bool> ValidationCheckAsync(string userId);
    
}