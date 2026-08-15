namespace Busines_Layer.Dtos.SnapShotDto;

public class SnapShotItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public long UnitPrice { get; set; }

    public int Count { get; set; }
    public decimal TotalPrice => UnitPrice * (1 - Discount / 100m) * Count;
    public decimal Discount { get; set; }
    public Guid SnapShotId { get; set; }
}