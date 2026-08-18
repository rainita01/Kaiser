using AutoMapper;
using Busines_Layer.Dtos.SnapShotDto;
using Data_Layer.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Busines_Layer.Repository.Sanpshot;

public class SnapshotRepo(Context context,ILogger<SnapshotRepo> logger,IMapper mapper) : ISnapshotRepo
{
    public async Task<ActionResult> RemoveAsync(string authority)
    {
        var result = await context.SnapShots.FirstOrDefaultAsync(e => e.Authority == authority);

        if (result == null)
        {
            logger.LogWarning("snapshot not found:{authority}",authority);
            return ActionResult.Failed("snap shot not found");
        }

        try
        {
            context.SnapShots.Remove(result);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e,"couldent delete snapshot:{id}",result.Id);

            return ActionResult.Failed("couldent delete snapshot");
        }

    }

    public async Task<SnapShotDto?> GetAsync(string authority)
    {
        return mapper.Map<SnapShotDto>(await context.SnapShots.Include(e=>e.Items).FirstOrDefaultAsync(e => e.Authority == authority));
    }
}