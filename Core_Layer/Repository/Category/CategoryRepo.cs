using AutoMapper;
using AutoMapper.QueryableExtensions;
using Busines_Layer.Dtos.Category;
using Busines_Layer.Services.TextServices;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Busines_Layer.Repository.Category;

public class CategoryRepo(ILogger<CategoryRepo> logger,Context context,IMapper mapper,TextServices textServices) :ICategoryRepo
{
    public async Task<ActionResult> AddAsync(AddCategoryDto dto)
    {
        try
        {
            if (await context.Categories.AnyAsync(e => e.Name == dto.Name))
            {
                logger.LogInformation("this category already exists name: {name}",dto.Name);
                return ActionResult.Failed("همچین دسته بندی ای وجود دارد");
            }

            var category = mapper.Map<Data_Layer.Entities.Category>(dto);
            category.Slug = textServices.GenerateSlug(category.Name);
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e,"Error while adding category: {@dto}",dto);
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> RemoveAsync(int id)
    {
        try
        {
            var category = await GetCategoryByIdAsync(id);
            if (category == null)
            {
                logger.LogWarning("while removing category with id:{id}",id);
                return ActionResult.Failed("دسته بندی پیدا نشد!...");
            }
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while deleting category: {id}", id);
            return ActionResult.Failed(e.Message);
        }
        
    }

    public async Task<ActionResult> UpdateAsync(UpdateCategoryDto dto)
    {
        try
        {
            var category = await GetCategoryByIdAsync(dto.Id);
            if (category == null)
            {
                logger.LogWarning("while updating category with id:{id}", dto.Id);
                return  ActionResult.Failed("دسته بندی پیدا نشد دوباره امتحان کنید");
            }

            category.Name = dto.Name ?? category.Name;
            category.Slug = textServices.GenerateSlug(category.Name);
            category.Keywords = dto.Keywords ?? category.Keywords;
            category.MetaDescription = dto.MetaDescription ?? category.MetaDescription;
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while updating category: {@dto}", dto);
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        try
        {
            return await context.Categories
                .AsNoTracking()
                .ProjectTo<CategoryDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while get categories");
            throw;
        }
    }

    private async Task<Data_Layer.Entities.Category?> GetCategoryByIdAsync(int id)
    {
        try
        {
            return await context.Categories.FirstOrDefaultAsync(e => e.Id == id);
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while get category:{id}",id);
            throw;
        }
    }
}