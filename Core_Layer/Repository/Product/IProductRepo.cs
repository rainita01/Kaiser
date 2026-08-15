using Busines_Layer.Dtos.Product;
using Data_Layer.Entities;

namespace Busines_Layer.Repository.Product;

public interface IProductRepo
{
    public Task<ActionResult> AddAsync(AddProductDto dto);  
    public Task<ActionResult> UpdateAsync(UpdateProductDto dto);
    public Task<ActionResult> DeleteAsync(int id);
    public Task<ProductPageDto> GetProductPagesAsync(int page, int? pageSize, decimal? minPrice, decimal? maxPrice,SortProduct? sort, string? search, int? categoryId,bool? isbestSale,bool? haveCost);
    public Task<List<ProductCardDto>> GetBestSalesProducts(int? pageSize, int? pageNumber);
    public Task<UpdateProductDto> GetUpdateProductAsync(int id);
    public Task<ProductDto> GetProductAsync(int id);
    public Task<ActionResult> ExecuteProductQuantityCostAsync(int productId,int count);

}