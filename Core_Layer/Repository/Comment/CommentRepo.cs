using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core_Layer.Dtos.Comment;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;

namespace Core_Layer.Repository.Comment;

public class CommentRepo(Context context,IMapper mapper) : ICommentRepo
{
    public async Task<ActionResult> AddAsync(AddCommentDto model, string userId)
    {
        try
        {
       
            var comment = mapper.Map<Data_Layer.Entities.Comment>(model);
            comment.UserId = userId;
            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();

            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> DeleteAsync(int id)
    {
        try
        {
            var comment = await context.Comments.FindAsync(id);
            if (comment == null)
                return ActionResult.Failed("کامنت پیدا نشد");
            context.Comments.Remove(comment);
            await context.SaveChangesAsync();

            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<List<CommentDto>> GetAllAsync()
    {
        return await context.Comments.AsNoTracking()
            .OrderByDescending(e=>e.IsApproved)
            .ProjectTo<CommentDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<List<CommentDto>> ProductCommentsAsync(int productId,int? totalCounts,int pageNumber= 1)
    {   
        return await context.Comments
            .AsNoTracking()
            .Skip((pageNumber - 1) * 10)
            .Take(totalCounts ?? 10)
            .Where(e => e.IsApproved && e.ProductId == productId)
            .OrderBy(e=>e.SendDate)
            .ProjectTo<CommentDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ActionResult> ApproveOrDisApproveCommentAsync(ApproveOrDisApproveCommentDto dto)
    {
      
        try
        {
            var comment = await context.Comments.FirstOrDefaultAsync(e => e.Id == dto.Id);
            if (comment == null)
                return ActionResult.Failed("کامنت پیدا نشد");

            comment.IsApproved = dto.IsApprove;

            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }


    }
}