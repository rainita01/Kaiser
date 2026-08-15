using Busines_Layer.Dtos.SnapShotDto;

namespace Busines_Layer.Repository.Sanpshot;

public interface ISnapshotRepo
{
    Task<ActionResult> RemoveAsync(string authority);
    Task<SnapShotDto?> GetAsync(string authority);
}