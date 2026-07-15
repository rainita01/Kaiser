

namespace Data_Layer.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public SnapShot SnapShot { get; set; }
    public Guid SnapShotId { get; set; }
    public long Amount { get; set; }
    public string? Authority { get; set; }
    public string? RefId { get; set; }
    public PaymentState State { get; set; } // Requested, Succeeded, Failed
    public DateTime CreatedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public int? OrderId { get; set; }   
    public Order? Order { get; set; }

}

public enum PaymentState { Requested, Succeeded, Failed }