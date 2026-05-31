using System.ComponentModel.DataAnnotations;

namespace Data_Layer.Entities;

public class Image
{
    [Key]
    public int Id { get; set; }
    [MaxLength(250)]
    public string Name { get; set; }
    public bool IsMain { get; set; } = false;

    #region Relations

    public int ProductId { get; set; }
    public Product Product { get; set; }    


    #endregion

}