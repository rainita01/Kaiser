
using AutoMapper;
using Core_Layer.Dtos.ImageDto;
using Core_Layer.Services.ImageServices;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Transaction = System.Transactions.Transaction;

namespace Core_Layer.Repository.Image;

public class ImageRepo(ILogger<ImageRepo> logger,Context context,ImageServices imageServices,IMapper mapper) : IImageRepo
{
    public async Task<ActionResult> RemoveAsync(int id)
    {
        try
        {
            var image = await GetImageAsync(id);
            if (image == null)
            {
                
                return ActionResult.Failed("عکس پیدا نشد...");
            }
            context.Images.Remove(image);
            var path = Path.Combine(imageServices.ContentRootPath,"Uploads", image.Name);
            if (File.Exists(path))
            {
               File.Delete(path);
            }
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
        using var transaction = Transaction.Current;

        try
        {
            if (!imageServices.ValidateExtension(image.FileName))
            {
                return ActionResult.Failed("فایل پسوند عکس ندارد...");
            }

            if (productId == 0)
            {
                return ActionResult.Failed("خطا در پروداکت ای دی");
            }
            var imageName = await imageServices.SaveImageAndGiveName(image);
            await context.Images.AddAsync(new Data_Layer.Entities.Image() {  Name = imageName, ProductId = productId });
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
        try
        {
            var image = await context.Images.FirstOrDefaultAsync(e => e.Id == id);
            return mapper.Map<ImageDto>(image);
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while getting image: {id}",id);
            throw;
        }

    }

    public async Task<string?> GetFirstImageAsync(int productId)
    {
        try
        {
            var image = await context.Images.FirstOrDefaultAsync(e => e.ProductId == productId);
            return image?.Name;
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while getting first image for product:{productId}",productId);
            throw;
        }
    }
    private async Task<Data_Layer.Entities.Image?> GetImageAsync(int id)
    {
        try
        {
            return await context.Images.FirstOrDefaultAsync(e => e.Id == id);
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while getting image by id : {id}",id);
            throw;
        }
    }
}