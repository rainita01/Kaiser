using AutoMapper;
using Core_Layer.Dtos.ImageDto;
using Core_Layer.Services.ImageServices;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;
using Data_Layer.Entities;
namespace Core_Layer.Repository.Image;

public class ImageRepo(Context context,ImageServices imageServices,IMapper mapper) : IImageRepo
{
    public async Task<ActionResult> RemoveAsync(int id)
    {
        try
        {
            var image = await GetImageAsync(id);
            if (image == null)
                return ActionResult.Failed("عکس پیدا نشد...");
            context.Images.Remove(image);
            await context.SaveChangesAsync();

            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> AddAsync(AddImageDto image)
    {
       
        try
        {
            if (!imageServices.ValidateExtension(image.Image.FileName))
            {
                return ActionResult.Failed("فایل پسوند عکس ندارد...");
            }
            var imageName = await imageServices.SaveImageAndGiveName(image.Image);
            await context.Images.AddAsync(new Data_Layer.Entities.Image()
                { IsMain = image.IsMain, Name = imageName, ProductId = image.ProductId });
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ImageDto> GetByIdAsync(int id)
    {
        var image = await context.Images.FirstOrDefaultAsync(e => e.Id == id);
        return mapper.Map<ImageDto>(image);

    }

    private async Task<Data_Layer.Entities.Image?> GetImageAsync(int id)
    {
        return await context.Images.FirstOrDefaultAsync(e => e.Id == id);
    }
}