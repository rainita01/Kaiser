using AutoMapper;
using Core_Layer.Dtos.ImageDto;
using Core_Layer.Dtos.Product;
using Core_Layer.Repository.Image;
using Core_Layer.Repository.Visitors;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;

namespace Core_Layer.Repository.Product;

public class ProductRepo(Context context,IMapper mapper, IImageRepo imageRepo,IViewsRepo viewsRepo) : IProductRepo
{
    public async Task<ActionResult> AddAsync(AddProductDto dto)
    {
        try
        {
            var model = mapper.Map<Data_Layer.Entities.Product>(dto);
            var images = new List<Data_Layer.Entities.Image>();
            foreach (var image in dto.Images)
            {
                await imageRepo.AddAsync(image);
            }

            model.Slug = GenerateSlug(dto.Name);
            await context.AddAsync(model);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> UpdateAsync(UpdateProductDto dto)
    {
        try
        {
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> DeleteAsync(int id)
    {
        try
        {
            var product = await GetProduct(id);
            if (product == null)
                return ActionResult.Failed($"محصول پیدا نشده است ای دی {id}");
            
            context.Remove(product);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public Task<List<ProductDto>> GetProductPagesAsync(int pageNumber)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductDto>> GetProductPagesAsync(int pageNumber, int categoryId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProductDto>> GetProductPagesAsync(int pageNumber, string search)
    {
        throw new NotImplementedException();
    }

    public async Task<ProductDto> GetProductAsync(int id)
    {
        var product = await context.Products.Include(e=>e.Images).FirstOrDefaultAsync(e => e.Id == id);
        var productDto =  mapper.Map<ProductDto>(product);
        productDto.Images = mapper.Map<List<ImageDto>>(product.Images);
        productDto.Views = await viewsRepo.GetPageViewsCount(id);
        return productDto;

    }

    private async Task<Data_Layer.Entities.Product?> GetProduct(int id)
    {
        return await context.Products.FirstOrDefaultAsync(e => e.Id == id);
    }
    private string GenerateSlug(string title)
    {
        if (string.IsNullOrEmpty(title))
            return "";

        // تبدیل به حروف کوچک
        var slug = title.ToLower();

        // جایگزینی فاصله با خط تیره
        slug = slug.Replace(" ", "-");

        // حذف کاراکترهای خاص
        slug = slug
            .Replace("?", "")
            .Replace("!", "")
            .Replace("،", "")
            .Replace(".", "")
            .Replace(":", "")
            .Replace(";", "");

        return slug;
    }
}