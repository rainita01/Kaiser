using Microsoft.AspNetCore.Http;

namespace Busines_Layer.Dtos.ImageDto;

public class AddImageDto
{
    public IFormFile Image { get; set; }
    public int? ProductId { get; set; }  
}