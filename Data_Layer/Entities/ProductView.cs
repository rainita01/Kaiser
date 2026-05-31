using System.ComponentModel.DataAnnotations;

namespace Data_Layer.Entities;

public class ProductView
{
    [Key]
    public long Id { get; set; }
    public DateTime ViewAt { get; set; }
    public string IpAddress { get; set; }
    public string SesstionId { get; set; }  


    #region Relations   
    public int ProductId { get; set; }
    public Product Product { get; set; }

    #endregion


}