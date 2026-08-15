namespace Busines_Layer.Services.GetServices;

public interface IGetCountServices
{
    Task<int> GetProductsCountAsync();
    Task<int> GetProductsWithDiscountCountAsync();
    Task<int> GetProductsWithBestSalesCountAsync();
    Task<int> GetCommentsCountAsync();
    Task<int> GetApprovedCommentsCountAsync();
    Task<int> GetVisitsCountAsync();
    Task<int> GetMessagesCountAsync();
    Task<int> GetUsersCountAsync();
    Task<int> GetCategoryCountAsync();
}