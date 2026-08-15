using Busines_Layer.Dtos.Comment;

namespace Busines_Layer.Repository.Comment;

public interface ICommentRepo
{
    public Task<ActionResult> AddAsync(AddCommentDto model,string userId);
    public Task<ActionResult> DeleteAsync(int id);

    public Task<List<CommentDto>> GetAllAsync();
    public Task<List<CommentDto>> ProductCommentsAsync(int productId, int? totalCounts, int pageNumber = 1);
    public Task<ActionResult> ApproveOrDisApproveCommentAsync(ApproveOrDisApproveCommentDto dto);

}