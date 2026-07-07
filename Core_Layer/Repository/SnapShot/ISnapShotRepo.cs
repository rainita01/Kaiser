using Core_Layer.Dtos.SnapShotDto;

namespace Core_Layer.Repository.SnapShot;

public interface ISnapShotRepo
{
    public Task<ActionResult> AddAsync(SnapShotDto dto);
    public Task<ActionResult> UpdateStateAsync(SnapShotUpdateStateDto dto);
}