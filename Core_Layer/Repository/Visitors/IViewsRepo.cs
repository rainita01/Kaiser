using Core_Layer.Dtos.ViewsDto;
using Microsoft.AspNetCore.Mvc;

namespace Core_Layer.Repository.Visitors;

public interface IViewsRepo
{
    public Task<ActionResult> AddAsync(AddViewDto dto);
    public Task<long> GetPageViewsCount(int pageId);

}