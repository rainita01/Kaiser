using Core_Layer.Dtos.ImageDto;

namespace Core_Layer.Dtos.Product;

public class ProductDto
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string MetaDescription { get; set; }
    public string Slug { get; set; }    
    public string KeyWords { get; set; }
    public long Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public long Views { get; set; }
    public int DiscountPercent { get; set; }
    public bool IsBestSell { get; set; }
    public List<ImageDto.ImageDto> Images { get; set; }
    public DateTime CreateTime { get; set; }

}