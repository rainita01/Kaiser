using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core_Layer.Dtos.Category;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;

namespace Core_Layer.Repository.Category;

public class CategoryRepo(Context context,IMapper mapper) :ICategoryRepo
{
    public async Task<ActionResult> AddAsync(string name)
    {
        try
        {
            if (await context.Categories.AnyAsync(e=>e.Name == name))
                return ActionResult.Failed("همچین دسته بندی ای وجود دارد");
            
            await context.Categories.AddAsync(new Data_Layer.Entities.Category() { Name = name });
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> RemoveAsync(int id)
    {
        try
        {
            var category = await GetCategoryByIdAsync(id);
            if (category == null)
                return ActionResult.Failed("دسته بندی پیدا نشد!...");
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
        
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        return await context.Categories
            .AsNoTracking()
            .ProjectTo<CategoryDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    private async Task<Data_Layer.Entities.Category?> GetCategoryByIdAsync(int id)
    {
        return await context.Categories.FirstOrDefaultAsync(e => e.Id == id);
    }
}