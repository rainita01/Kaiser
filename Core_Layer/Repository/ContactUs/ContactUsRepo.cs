using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core_Layer.Dtos.ContactUsDto;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core_Layer.Repository.ContactUs;

public class ContactUsRepo(Context context,IMapper mapper) : IContactUsRepo
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
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> Remove(int id)
    {
        try
        {
            var model = await context.ContactMessages.FindAsync(id);
            if (model == null)
                return ActionResult.Failed("مدل پیدا نشد ");
            context.ContactMessages.Remove(model);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<List<ContactUsDto>> Get()
    {
       return await context.ContactMessages
           .AsNoTracking()
           .OrderByDescending(e=>e.SendTime)
           .ProjectTo<ContactUsDto>(mapper.ConfigurationProvider)
           .ToListAsync();
    }
}