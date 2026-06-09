using AutoMapper;
using Core_Layer.Dtos.ImageDto;
using Core_Layer.Services.ImageServices;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;
using Data_Layer.Entities;
using Microsoft.AspNetCore.Http;

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

    public async Task<ActionResult> AddAsync(IFormFile image,int productId)
    {
       
        try
        {
            if (!imageServices.ValidateExtension(image.FileName))
            {
                return ActionResult.Failed("فایل پسوند عکس ندارد...");
            }
            var imageName = await imageServices.SaveImageAndGiveName(image);
            await context.Images.AddAsync(new Data_Layer.Entities.Image() {  Name = imageName, ProductId = productId });
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

    public async Task<string?> GetFirstImageAsync(int productId)
    {
        var image =  await context.Images.FirstOrDefaultAsync(e => e.ProductId == productId);
        return image?.Name;
    }
    private async Task<Data_Layer.Entities.Image?> GetImageAsync(int id)
    {
        return await context.Images.FirstOrDefaultAsync(e => e.Id == id);
    }
}