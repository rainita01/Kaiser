using Core_Layer.Dtos.Product;
using Data_Layer.Entities;

namespace Core_Layer.Repository.Product;

public interface IProductRepo
{
    public Task<ActionResult> AddAsync(AddProductDto dto);
    public Task<ActionResult> UpdateAsync(UpdateProductDto dto);
    public Task<ActionResult> DeleteAsync(int id);
    public Task<List<ProductCardDto>> GetProductPagesAsync(int page, int? pageSize, decimal? minPrice, decimal? maxPrice,SortProduct? sort, string? search, int? categoryId);
    public Task<UpdateProductDto> GetUpdateProductAsync(int id);
    public Task<ProductDto> GetProductAsync(int id);

}