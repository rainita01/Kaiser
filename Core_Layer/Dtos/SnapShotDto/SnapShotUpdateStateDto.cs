using Data_Layer.Entities;

namespace Busines_Layer.Dtos.SnapShotDto;

public class SnapShotUpdateStateDto
{
    public Guid Id { get; set; }
    public SnapShotState SnapShotState { get; set; }
     
}