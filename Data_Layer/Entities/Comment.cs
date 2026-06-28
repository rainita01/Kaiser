using System.ComponentModel.DataAnnotations;

namespace Data_Layer.Entities;

public class Comment : BaseEntity
{
    [MaxLength(500)]
    [Required]
    public string Text { get; set; }
    public bool IsApproved { get; set; }
    public DateTime SendDate { get; set; } 
    #region Relations

    public string UserId { get; set; }
    public User User { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int? ParentId { get; set; }
    public Comment? Parent { get; set; }
    public List<Comment>? Replays { get; set; }

    #endregion
}