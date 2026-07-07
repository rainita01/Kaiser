using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data_Layer.Entities;

public class OrderItem : BaseEntity 
{
    public string ProductName { get; set; }
    public long UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public int Count { get; set; }

    #region Relations

    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; }

    #endregion
}