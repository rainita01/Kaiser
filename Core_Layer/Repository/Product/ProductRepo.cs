
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Busines_Layer.Dtos.Product;
using Busines_Layer.Repository.Image;
using Busines_Layer.Repository.Visitors;
using Busines_Layer.Services.TextServices;
using Data_Layer.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Busines_Layer.Repository.Product;

public class ProductRepo(ILogger<ProductRepo> logger,Context context,IMapper mapper, IImageRepo imageRepo,IViewsRepo viewsRepo,TextServices textServices) : IProductRepo
{
    public async Task<ActionResult> AddAsync(AddProductDto dto)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var model = mapper.Map<Data_Layer.Entities.Product>(dto);
            model.Slug = textServices.GenerateSlug(dto.Name);

            await context.Products.AddAsync(model);
            await context.SaveChangesAsync();
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
            logger.LogInformation("new product created: {@dto}",dto);
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while adding new product {@dto}",dto);
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
            product.Length = dto.Length ?? product.Length;
            product.BoxTypeId = dto.BoxTypeId ?? product.BoxTypeId;
            product.Height = dto.Height ?? product.Height;
            product.TotalWeight = dto.TotalWeight ?? product.TotalWeight;
            product.Width = dto.Width ?? product.Width;
           
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while updating product:{id}",dto.Id);
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
            logger.LogError(e,"error while removing product:{id}",id);
            return ActionResult.Failed(e.Message);
        }
    }
  
    public async Task<ProductPageDto> GetProductPagesAsync(
        int page,
        int? pageSize,
        decimal? minPrice,
        decimal? maxPrice,
        SortProduct? sort,
        string? search,
        int? categoryId,
        bool? isBestSale,
        bool? haveCost
            )
    {
        page = Math.Max(page, 1);
        var actualPageSize = pageSize ?? 10;
        var skip = (page - 1) * actualPageSize;


        var query = context.Products.AsNoTracking();

        if (categoryId.HasValue)
            query = query.Where(x => x.CategoryId == categoryId);

        if (minPrice.HasValue)
            query = query.Where(x => x.Price >= minPrice);

        if (maxPrice.HasValue)
            query = query.Where(x => x.Price <= maxPrice);

        if (isBestSale.HasValue)
            query = query.Where(x => x.IsBestSell == isBestSale);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => EF.Functions.Like(x.Name, $"%{search}%"));

        if (haveCost == true)
            query = query.Where(x => x.DiscountPercent > 0);

        query = sort switch
        {
            SortProduct.MostViewed => query.OrderByDescending(e => e.ProductViews!.Count),
            SortProduct.PriceAsc => query.OrderBy(e => e.Price),
            SortProduct.PriceDesc => query.OrderByDescending(e => e.Price),
            SortProduct.Newest => query.OrderByDescending(e=>e.CreateTime),

            _ => query.OrderBy(e => e.Id)
        };
        var count =Math.Ceiling((double)query.Count() / actualPageSize) ;
        try
        {
            var items = await query
                .Skip(skip)
                .Take(actualPageSize)
                .ProjectTo<ProductCardDto>(mapper.ConfigurationProvider)
                .ToListAsync();
            return new ProductPageDto() { ProductCards = items,PageCount = count};
        }
        catch (Exception e)    
        {
            logger.LogError(e,"error while getting products pages");
            throw;
        }
    }

    public async Task<List<ProductCardDto>> GetBestSalesProducts(int? pageSize,int? pageNumber)
    {

        int actualPageSize = pageSize ?? 10;
        int actualPageNumber = pageNumber ?? 1;
        actualPageSize = Math.Max(1, actualPageSize);
        actualPageNumber = Math.Max(1, actualPageNumber);

        if (actualPageSize > 100) actualPageSize = 100;

        var query = context.Products
            .AsNoTracking()
            .Where(e => e.IsBestSell);

        //var totalCount = await query.CountAsync();

        try
        {
            var items = await query
                .OrderByDescending(e => e.CreateTime)
                .Skip((actualPageNumber - 1) * actualPageSize)
                .Take(actualPageSize)
                .ProjectTo<ProductCardDto>(mapper.ConfigurationProvider)
                .ToListAsync();

            return items;
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while getting best sales products");
            throw;
        }
    }
    public async Task<ProductDto> GetProductAsync(int id)
    {
        try
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
        catch (Exception e)
        {
            logger.LogError(e,"error while get product:{id}",id);
            throw;
        }

    }

    public async Task<ActionResult> ExecuteProductQuantityCostAsync(int productId, int count)
    {
        try
        {
            var affected = await context.Products
                .Where(p => p.Id == productId && p.StockQuantity >= count)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.StockQuantity, p => p.StockQuantity - count));
            if (affected == 0)
            {
                logger.LogWarning("tried to executed product:{id} but quantity was lower than needed",productId);
                return ActionResult.Failed("موجودی کمتر از انتظار بود");
            }
                
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while executing product quantity in method");
            throw;
        }
    }

    public async Task<UpdateProductDto> GetUpdateProductAsync(int id)
    {
        try
        {
            var product = await context.Products
                .Include(e => e.Images)
                .FirstOrDefaultAsync(e => e.Id == id);
            var productDto = mapper.Map<UpdateProductDto>(product);
            return productDto;
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while getting product:{id} for updating",id);
            throw;
        }
    }
    private async Task<Data_Layer.Entities.Product?> GetProduct(int id)
    {
        return await context.Products.FirstOrDefaultAsync(e => e.Id == id);
    }
  
}