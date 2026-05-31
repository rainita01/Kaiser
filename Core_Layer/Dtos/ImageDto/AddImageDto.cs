using Microsoft.AspNetCore.Http;

namespace Core_Layer.Dtos.ImageDto;

public class AddImageDto
{
    public int ProductId { get; set; }    
    public IFormFile Image { get; set; }
    public bool IsMain { get; set; }    

}