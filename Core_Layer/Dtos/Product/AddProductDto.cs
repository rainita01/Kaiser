using Busines_Layer.Dtos.ImageDto;
using Microsoft.AspNetCore.Http;

namespace Busines_Layer.Dtos.Product;

public class AddProductDto
{
    public string Name { get; set; }    
    public string Description { get; set; }
    public string MetaDescription { get; set; }
    public string KeyWords { get; set; }
    public long Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public int DiscountPercent { get; set; }
    public bool IsBestSell { get; set; }
    public int Length { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public long TotalWeight { get; set; }
    public int BoxTypeId { get; set; }
    public List<IFormFile>? Images { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;

}