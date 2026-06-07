using Core_Layer.Dtos.ImageDto;

namespace Core_Layer.Repository.Image;

public interface IImageRepo
{
    public Task<ActionResult> RemoveAsync(int id);
    public Task<ActionResult> AddAsync(AddImageDto image,int productId);
    public Task<ImageDto> GetByIdAsync(int id);
    public Task<ImageDto> GetFirstImageAsync(int productId);
}