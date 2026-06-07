namespace Core_Layer.Dtos.Product;

public class ProductCardDto
{
    public int Id { get; set; }     
    public string Name { get; set; }
    public string MetaDescription { get; set; }
    public string Slug { get; set; }
    public long Price { get; set; }
    public int CategoryId { get; set; }
    public long Views { get; set; }
    public int DiscountPercent { get; set; }
    public bool IsBestSell { get; set; }
    public ImageDto.ImageDto Image { get; set; }
}