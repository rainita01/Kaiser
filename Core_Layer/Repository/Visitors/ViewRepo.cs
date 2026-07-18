
using Core_Layer.Dtos.ViewsDto;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core_Layer.Repository.Visitors;

public class ViewRepo(ILogger<ViewRepo> logger,Context context) :IViewsRepo
{
    public async Task<ActionResult> AddAsync(AddViewDto dto)
    {
        try
        {
            var model = new ProductView()
            {
                IpAddress = dto.IpAddress,
                ProductId = dto.ProductId,
                SesstionId = dto.SesstionId,
                ViewAt = dto.ViewAt
            };
            await context.ProductViews.AddAsync(model);
            await context.SaveChangesAsync();

            logger.LogInformation("new view saved {@dto}",dto);
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<long> GetPageViewsCount(int pageId)
    {
        try
        {
            return await context.ProductViews.CountAsync(e => e.ProductId == pageId);
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while getting page views: {pageId}",pageId);
            throw;
        }
    }
}