using Core_Layer.Dtos.Category;

namespace Core_Layer.Repository.Category;

public interface ICategoryRepo
{
    public Task<ActionResult> AddAsync(AddCategoryDto dto);
    public Task<ActionResult> RemoveAsync(int id);
    public Task<ActionResult> UpdateAsync(UpdateCategoryDto dto);
    public Task<List<CategoryDto>> GetCategoriesAsync();
}