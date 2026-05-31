using Core_Layer.Dtos.Category;

namespace Core_Layer.Repository.Category;

public interface ICategoryRepo
{
    public Task<ActionResult> AddAsync(string name);
    public Task<ActionResult> RemoveAsync(int id);
    public Task<List<CategoryDto>> GetCategoriesAsync();
}