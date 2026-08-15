using AutoMapper;
using AutoMapper.QueryableExtensions;
using Busines_Layer.Dtos.ContactUsDto;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Busines_Layer.Repository.ContactUs;

public class ContactUsRepo(ILogger<ContactUsRepo> logger,Context context,IMapper mapper) : IContactUsRepo
{
    public async Task<ActionResult> Add(ContactUsDto contactUsDto)
    {
        try
        {
            await context.ContactMessages.AddAsync(mapper.Map<ContactMessage>(contactUsDto));
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while adding contactUs: {@dto}",contactUsDto);
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> Remove(int id)
    {
        try
        {
            var model = await context.ContactMessages.FindAsync(id);
            if (model == null)
            {
                logger.LogWarning("contact did not found while getting contact us message with id: {id}",id);
                return ActionResult.Failed("مدل پیدا نشد ");
            }
            context.ContactMessages.Remove(model);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e, "error while deleting contactUs: {id}",id);
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<List<ContactUsDto>> Get()
    {
        try
        {
            return await context.ContactMessages
                .AsNoTracking()
                .OrderByDescending(e => e.SendTime)
                .ProjectTo<ContactUsDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while get contact us list");
            throw;
        }
    }
}