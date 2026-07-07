using AutoMapper;
using Core_Layer.Dtos.SnapShotDto;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;

namespace Core_Layer.Repository.SnapShot;
using Data_Layer.Entities;
public class SnapShotRepo(Context context,IMapper mapper) : ISnapShotRepo
{
    public async Task<ActionResult> AddAsync(SnapShotDto dto)
    {
        try
        {
            var snapShot = mapper.Map<SnapShot>(dto);
            await context.SnapShots.AddAsync(snapShot);
            await context.SaveChangesAsync();
           return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> UpdateStateAsync(SnapShotUpdateStateDto dto)
    {
        try
        {
            var snapShot = await context.SnapShots.FindAsync(dto.Id);
            if (snapShot == null)
            {
                return ActionResult.Failed("اسنپ شات پیدا نشد");
            }
            snapShot.State = dto.SnapShotState;
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

}