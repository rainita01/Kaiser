using Core_Layer.Dtos.Product;
using Data_Layer.Entities;

namespace Core_Layer.Repository.Product;

public interface IProductRepo
{
    public Task<ActionResult> AddAsync(AddProductDto dto);
    public Task<ActionResult> UpdateAsync(UpdateProductDto dto);
    public Task<ActionResult> DeleteAsync(int id);
    public Task<List<ProductDto>> GetProductPagesAsync(int pageNumber);
    public Task<List<ProductDto>> GetProductPagesAsync(int pageNumber, int? categoryId);
    public Task<List<ProductDto>> GetProductPagesAsync(int pageNumber, string search);
    public Task<ProductDto> GetProductAsync(int id);
}