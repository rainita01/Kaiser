using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core_Layer.Dtos.ImageDto;
using Core_Layer.Dtos.Product;
using Core_Layer.Repository.Image;
using Core_Layer.Repository.Visitors;
using Core_Layer.Services.TextServices;
using Data_Layer.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core_Layer.Repository.Product;

public class ProductRepo(Context context,IMapper mapper, IImageRepo imageRepo,IViewsRepo viewsRepo,TextServices textServices) : IProductRepo
{
    public async Task<ActionResult> AddAsync([FromBody] AddProductDto dto)
    {
        try
        {
            var model = mapper.Map<Data_Layer.Entities.Product>(dto);

            model.Slug = textServices.GenerateSlug(dto.Name);
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
            var product = await GetProduct(dto.Id);
            if (product == null)
                return ActionResult.Failed("محصول پیدا نشد!");


            product.Name = dto.Name ?? product.Name;
            product.Slug = textServices.GenerateSlug(product.Name);
            product.Description = dto.Description ?? product.Description;
            product.MetaDescription = dto.MetaDescription ?? product.MetaDescription;
            product.KeyWords = dto.KeyWords ?? product.KeyWords;

            
            product.Price = dto.Price ?? product.Price;
            product.StockQuantity = dto.StockQuantity ?? product.StockQuantity;
            await context.SaveChangesAsync();
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

    public async Task<List<ProductDto>> GetProductPagesAsync(int pageNumber)
    {
        return await context.Products
            .AsNoTracking()
            .Skip((pageNumber - 1) * 10)
            .Take(10)
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<List<ProductDto>> GetProductPagesAsync(int pageNumber, int? categoryId)
    {
        return await context.Products
            .AsNoTracking()
            .Where(e => e.CategoryId == categoryId)
            .Skip((pageNumber - 1) * 10)
            .Take(10)
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<List<ProductDto>> GetProductPagesAsync(int pageNumber, string search)
    {
        return await context.Products
            .AsNoTracking()
            .Where(e => e.Name.Contains(search))
            .Skip((pageNumber - 1) * 10)
            .Take(10)
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
            .ToListAsync();
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
  
}