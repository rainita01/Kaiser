using Core_Layer.Dtos.ImageDto;
using Microsoft.AspNetCore.Http;

namespace Core_Layer.Repository.Image;

public interface IImageRepo
{
    public Task<ActionResult> RemoveAsync(int id);
    public Task<ActionResult> AddAsync(IFormFile image,int productId);
    public Task<ImageDto> GetByIdAsync(int id);
    public Task<string?> GetFirstImageAsync(int productId);
}