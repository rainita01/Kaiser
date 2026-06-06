using Microsoft.AspNetCore.Http;

namespace Core_Layer.Dtos.ImageDto;

public class AddImageDto
{
    public IFormFile Image { get; set; }

}