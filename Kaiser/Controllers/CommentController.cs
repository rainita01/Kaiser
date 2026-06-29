using System.Security.Claims;
using Core_Layer.Dtos.Comment;
using Core_Layer.Repository.Comment;
using Microsoft.AspNetCore.Mvc;

namespace Kaiser.Controllers;

public class CommentController(ICommentRepo commentRepo) : ControllerBase
{

    [HttpGet("CommentManager/Comments")]
  public async Task<IActionResult> GetComments()
  { 
      var result = await commentRepo.GetAllAsync();
      return Ok(result);
  }
    [HttpGet("Products/Detail/Comments")]
    public async Task<IActionResult> GetProductComments(
        [FromQuery] int productId,
        [FromQuery]int? totalComments,
        [FromQuery]int pageNumber = 1)
    {

        var result = await commentRepo.ProductCommentsAsync(productId, totalComments, pageNumber);
        return Ok(result);

    }
  
    [HttpPost("AddComment")]
  public async Task<IActionResult> AddComment([FromBody]AddCommentDto dto)
  {
      if (!ModelState.IsValid)
          return BadRequest("مدل شما کامل نیست");
        

      var user = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (user == null)
          return BadRequest("لطفا اول وارد شوید");
      var result = await commentRepo.AddAsync(dto, user);

      if (result.Success)
          return Ok();
      return BadRequest(result.Message);
  }
    [HttpDelete("CommentManager/DeleteComment")]
  public async Task<IActionResult> RemoveComment(int commentId)
  {
      var result =await commentRepo.DeleteAsync(commentId);
      if (result.Success)
          return Ok();
      return BadRequest(result.Message);
  }

    [HttpPut("CommentManager/ApproveOrDisApproveComment")]
  public async Task<IActionResult> ApproveOrDissApproveComment([FromBody]ApproveOrDisApproveCommentDto dto)
  {
      var result = await commentRepo.ApproveOrDisApproveCommentAsync(dto);
      if (result.Success)
          return Ok();
      return BadRequest(result.Message);
    }

}