using System.ComponentModel.DataAnnotations;

namespace Data_Layer.Entities;

public class Review : BaseEntity
{
    [MaxLength(500)]
    [Required]
    public string Comment { get; set; }
    public bool IsApproved { get; set; }    



    #region Relations

    public string UserId { get; set; }
    public User User { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    

    #endregion
}