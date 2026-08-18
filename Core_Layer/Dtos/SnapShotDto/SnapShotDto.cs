using Data_Layer.Entities;

namespace Busines_Layer.Dtos.SnapShotDto;

public class SnapShotDto
{
    public Guid? Id { get; set; }
    public long ShippingCost { get; set; }  // هزینه ارسال
    public long TotalPrice { get; set; }
    public SnapShotState State { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? RefId { get; set; }
    public string Authority { get; set; }
    public DateTime ExpiresAt => DateTime.Now + TimeSpan.FromMinutes(15);
    public string UserId { get; set; }
    public int AddressId { get; set; }  

    public List<SnapShotItemDto> Items { get; set; }
}