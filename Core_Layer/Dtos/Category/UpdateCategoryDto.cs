namespace Core_Layer.Dtos.Category;

public class UpdateCategoryDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? MetaDescription { get; set; }
    public string? Keywords { get; set; }
}