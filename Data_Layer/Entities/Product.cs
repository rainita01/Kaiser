using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data_Layer.Entities;

public class Product : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    [Required]
    [MaxLength(500)]
    public string Description { get; set; }
    [Required]
    [MaxLength(170)]
    public string MetaDescription { get; set; }
    [Required]
    public string Slug { get; set; }
    [Required]
    public string KeyWords { get; set; }
    [Required]
    public long Price { get; set; }
    [Required]
    public int StockQuantity { get; set; }
    public int DiscountPercent { get; set; }
    public bool IsBestSell { get; set; }
    public DateTime CreateTime { get; set; }

    #region Relations

    public List<ProductView>? ProductViews { get; set; } 
    public List<Image>? Images { get; set; } 
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public List<Comment> Comments { get; set; }

    #endregion
}