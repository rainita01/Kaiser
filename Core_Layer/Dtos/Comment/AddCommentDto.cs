namespace Core_Layer.Dtos.Comment;

public class AddCommentDto
{
    public string Text { get; set; }
    public int? ParentId { get; set; }
    public int ProductId { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
}