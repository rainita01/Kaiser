namespace Data_Layer.Entities;

public class SnapShotItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public long UnitPrice { get; set; }

    public int Count { get; set; }

    public decimal Discount { get; set; }

        

    public Guid SnapShotId { get; set; }
    public SnapShot SnapShot { get; set; }  
}
