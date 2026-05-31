using System.ComponentModel.DataAnnotations;

namespace Data_Layer.Entities;

public class Category 
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(250)]
    public string Name { get; set; }

    public string Slug { get; set; }
    public string MetaDescription { get; set; }
    public string Keywords { get; set; }    


    #region Relations

    public List<Product>? Products { get; set; } 

    #endregion
}