namespace Core_Layer.Dtos.ImageDto;

public class ImageDto
{
    public int Id { get; set; }
    public bool IsMain { get; set; }    
    public int ProductId { get; set; }  
    public string ImageName { get; set; }
}