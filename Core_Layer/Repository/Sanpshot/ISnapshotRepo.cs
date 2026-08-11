using Core_Layer.Dtos.SnapShotDto;
namespace Core_Layer.Repository.Sanpshot;

public interface ISnapshotRepo
{
    Task<ActionResult> RemoveAsync(string authority);
    Task<SnapShotDto?> GetAsync(string authority);
}