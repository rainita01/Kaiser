namespace Core_Layer.Dtos.Comment;

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string Writer { get; set; }
    public int ProductId { get; set; }  
    public int? ParentId { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreateDate { get; set; }    

}