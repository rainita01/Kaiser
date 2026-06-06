using Core_Layer.Dtos.ImageDto;

namespace Core_Layer.Dtos.Product;

public class AddProductDto
{
    public string Name { get; set; }    
    public string Description { get; set; }
    public string MetaDescription { get; set; }
    public string KeyWords { get; set; }
    public long Price { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
    public List<AddImageDto> Images { get; set; }   

}