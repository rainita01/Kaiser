using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core_Layer.Dtos.Product;
using Core_Layer.Repository.Image;
using Core_Layer.Repository.Visitors;
using Core_Layer.Services.TextServices;
using Data_Layer.Context;

using Microsoft.EntityFrameworkCore;

namespace Core_Layer.Repository.Product;

public class ProductRepo(Context context,IMapper mapper, IImageRepo imageRepo,IViewsRepo viewsRepo,TextServices textServices) : IProductRepo
{
    public async Task<ActionResult> AddAsync(AddProductDto dto)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var model = mapper.Map<Data_Layer.Entities.Product>(dto);
            model.Slug = textServices.GenerateSlug(dto.Name);

            await context.Products.AddAsync(model);
            await context.SaveChangesAsync(); // model.Id ساخته شد
            if (dto.Images != null)
            {
                foreach (var image in dto.Images)
                {

                    var result = await imageRepo.AddAsync(image, model.Id);
                    if (!result.Success)
                    {
                        return ActionResult.Failed(result.Message!);
                    }
                }

                await context.SaveChangesAsync();
                
            }

            await transaction.CommitAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
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
            product.DiscountPercent = dto.DiscountPercent ?? product.DiscountPercent;
            product.IsBestSell = dto.IsBestSell ?? product.IsBestSell;
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
  
    public async Task<List<ProductCardDto>> GetProductPagesAsync(
        int page,
        int? pageSize,
        decimal? minPrice,
        decimal? maxPrice,
        SortProduct? sort,
        string? search,
        int? categoryId)
    {
        var actualPageSize = pageSize ?? 10;
        var skip = (page - 1) * actualPageSize;


        var query = context.Products
            .AsNoTracking()
            .Where(e => (categoryId == null || e.CategoryId == categoryId) &&
                        (minPrice == null || e.Price >= minPrice) &&
                        (maxPrice == null || e.Price <= maxPrice) &&
                        (string.IsNullOrEmpty(search) || e.Name.Contains(search)));

        // اعمال مرتب‌سازی در دیتابیس
        query = sort switch
        {
            SortProduct.MostViewed => query.OrderByDescending(e => e.ProductViews!.Count),
            SortProduct.PriceAsc => query.OrderBy(e => e.Price),
            SortProduct.PriceDesc => query.OrderByDescending(e => e.Price),
            SortProduct.Newest => query.OrderByDescending(e=>e.CreateTime),
            _ => query.OrderBy(e => e.Id)
        };

        // اعمال صفحه‌بندی و ProjectTo
        return await query
            .Skip(skip)
            .Take(actualPageSize)
            .ProjectTo<ProductCardDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<List<ProductCardDto>> GetBestSalesProducts(int? pageSize,int? pageNumber)
    {

        // تنظیم مقادیر پیش‌فرض
        int actualPageSize = pageSize ?? 10;
        int actualPageNumber = pageNumber ?? 1;

        // اعتبارسنجی
        actualPageSize = Math.Max(1, actualPageSize);
        actualPageNumber = Math.Max(1, actualPageNumber);

        // جلوگیری از دریافت بیش از حد
        if (actualPageSize > 100) actualPageSize = 100;

        var query = context.Products
            .AsNoTracking()
            .Where(e => e.IsBestSell);

        //var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(e => e.CreateTime)
            .Skip((actualPageNumber - 1) * actualPageSize)
            .Take(actualPageSize)
            .ProjectTo<ProductCardDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return items;
    }
    public async Task<ProductDto> GetProductAsync(int id)
    {
        var product = await context.Products
            .AsNoTracking()
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (product == null)
            throw new NullReferenceException();
        product.Views = await viewsRepo.GetPageViewsCount(id);
        return product;

    }

    public async Task<UpdateProductDto> GetUpdateProductAsync(int id)
    {
        var product = await context.Products
            .Include(e => e.Images)
            .FirstOrDefaultAsync(e => e.Id == id);
        var productDto = mapper.Map<UpdateProductDto>(product);
        return productDto;
    }
    private async Task<Data_Layer.Entities.Product?> GetProduct(int id)
    {
        return await context.Products.FirstOrDefaultAsync(e => e.Id == id);
    }
  
}