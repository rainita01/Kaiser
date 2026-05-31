using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data_Layer.Entities;

public class CartItem : BaseEntity
{
    [Required]
    public int Quantity { get; set; }
    #region Relations

    public int CartId { get; set; }
    public Cart Cart { get; set; }
    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }
    #endregion
}