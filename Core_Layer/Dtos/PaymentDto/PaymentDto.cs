using Data_Layer.Entities;

namespace Busines_Layer.Dtos.PaymentDto;

public class PaymentDto
{
    public Guid Id { get; set; }
    public SnapShotDto.SnapShotDto SnapShot { get; set; }
    public long Amount { get; set; }
    public string? Authority { get; set; }
    public string? RefId { get; set; }
    public PaymentState State { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public int? OrderId { get; set; }
}