using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core_Layer.Dtos.Comment;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core_Layer.Repository.Comment;

public class CommentRepo(ILogger<CommentRepo> logger,Context context,IMapper mapper) : ICommentRepo
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
            logger.LogError(e,"user:{userId} tried to add comment but got error:{@model} " ,userId,model);
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
            logger.LogError(e,"Error while deleting comment: {id}",id);
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<List<CommentDto>> GetAllAsync()
    {
        try
        {
            return await context.Comments.AsNoTracking()
                .OrderByDescending(e => e.IsApproved)
                .ProjectTo<CommentDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while getting all comments");
            throw;
        }
    }

    public async Task<List<CommentDto>> ProductCommentsAsync(int productId,int? totalCounts,int pageNumber= 1)
    {
        try
        {
            return await context.Comments
                .AsNoTracking()
                .Skip((pageNumber - 1) * 10)
                .Take(totalCounts ?? 10)
                .Where(e => e.IsApproved && e.ProductId == productId)
                .OrderBy(e => e.SendDate)
                .ProjectTo<CommentDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e,"error while getting comments for product:{productId} with totalCounts:{counts}",productId,totalCounts);
            throw;
        }
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
            logger.LogError(e,"error while approve or disApprove {@dto} ",dto);
            return ActionResult.Failed(e.Message);
        }


    }
}