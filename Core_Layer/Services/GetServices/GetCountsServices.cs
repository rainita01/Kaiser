using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;

namespace Busines_Layer.Services.GetServices;

public class GetCountsServices(Context context) :IGetCountServices
{
    public async Task<int> GetProductsCountAsync()
    {
        return await context.Products.CountAsync();
    }

    public async Task<int> GetProductsWithDiscountCountAsync()
    {
        return await context.Products
            .CountAsync(p => p.DiscountPercent > 0);
    }

    public async Task<int> GetProductsWithBestSalesCountAsync()
    {
        return await context.Products
            .CountAsync(p => p.IsBestSell);
    }

    public async Task<int> GetCommentsCountAsync()
    {
        return await context.Comments.CountAsync();
    }

    public async Task<int> GetApprovedCommentsCountAsync()
    {
        return await context.Comments
            .CountAsync(c => c.IsApproved);
    }

    public async Task<int> GetVisitsCountAsync()
    {
        return await context.ProductViews.CountAsync();
    }

    public async Task<int> GetMessagesCountAsync()
    {
        return await context.ContactMessages.CountAsync();
    }

    public async Task<int> GetUsersCountAsync()
    {
        return await context.Users.CountAsync();
    }
    public async Task<int> GetCategoryCountAsync()
    {
        return await context.Categories.CountAsync();
    }
}