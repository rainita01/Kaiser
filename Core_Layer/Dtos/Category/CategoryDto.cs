using Busines_Layer.Dtos.ImageDto;
using Busines_Layer.Dtos.Product;

namespace Busines_Layer.Dtos.Category;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string MetaDescription { get; set; }
    public string Keywords { get; set; }
}