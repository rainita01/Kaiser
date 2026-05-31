

using AutoMapper;
using Core_Layer.Dtos.ViewsDto;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core_Layer.Repository.Visitors;

public class ViewRepo(Context context,IMapper mapper) :IViewsRepo
{
    public async Task<ActionResult> AddAsync(AddViewDto dto)
    {
        try
        {
            var model = mapper.Map<ProductView>(dto);
            await context.ProductViews.AddAsync(model);
            await context.SaveChangesAsync();


            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<long> GetPageViewsCount(int pageId)
    {
        return await context.ProductViews.CountAsync(e => e.ProductId == pageId);
    }
}